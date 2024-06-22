// These two functions are going to help us look at the state of the transformers
string GetNotebookString(object obj) {
    if (obj == null) {
        return "<null>";
    }

    if (obj is string) {
        return (string)obj;
    }

    if (obj is ITransformer trans) {
        return "{" +  
        trans.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.FieldType.Name != "IHost")
            .Select(f => {
                    object value = f.GetValue(trans);

                    if (value is null)
                    {
                        return f.Name + ": <null>";
                    }
                    return f.Name + ":" + GetNotebookString(value);
                })
            + "}";
    }

    if (obj is Type) {
        return ((Type)obj).Name;
    }

    if (obj is BitArray bitArr) {
        var sb = new StringBuilder();
        for (int i = 0; i < bitArr.Count; i++)
        {
            char c = bitArr[i] ? '1' : '0';
            sb.Append(c);
        }
        return sb.ToString();
    }

    if (obj is ValueTuple<string, string> tuple) {
        return "(" + GetNotebookString(tuple.Item1) + ", " + GetNotebookString(tuple.Item2) + ")";
    }

    if (obj is ValueTuple<string, string>[] tuples) {
        return "[" + Environment.NewLine + "\t" + string.Join($",{Environment.NewLine}\t", tuples.Select(t => GetNotebookString(t))) + "]";
    }

    if (obj is IEnumerable) {
        foreach (var o in (IEnumerable)obj) {
            return GetNotebookString(o) + ", ";
        }
    }

    return JsonConvert.SerializeObject(obj, Formatting.Indented);
}

// This function will help us visualize the transformer
IEnumerable<(string, string, string)> ReflectFields(object input) {
    if (input == null) {
        yield break;
    }

    var result = input.GetType()
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(f => f.FieldType.Name != "IHost")
            .Select(f => {
                    object value = f.GetValue(input);

                    if (value is null)
                    {
                        return (f.Name, f.FieldType.Name, null);
                    }
                    return (f.Name, f.FieldType.Name, GetNotebookString(value));
                });

    foreach (var r in result) {
        yield return r;
    }
}

Formatter.Register<ITransformer>((transformer, writer) => {
    // Write a title above the table with the type of object it was
    writer.Write("<h4>" + transformer.GetType().FullName + "</h4>");
    writer.Write("<table>");
    writer.Write("<thead><tr><th>Field</th><th>Type</th><th>Content</th></tr></thead>");
    foreach (var (field, type, content) in ReflectFields(transformer)) {
        writer.Write("<tr><th><strong>");
        writer.Write(field);
        writer.Write("</strong></th><td>");
        writer.Write(type);
        writer.Write("</td><td>");
        writer.Write(content);
        writer.Write("</td></tr>");
    }
    writer.Write("</table>");
}, "text/html");