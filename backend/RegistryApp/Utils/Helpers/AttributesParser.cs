namespace RegistryApp.Utils.Helpers {
    public static class AttributesParser {
        public static int? TryDefineInteger(Dictionary<string, string>? pairs, string key) {
            int softId;
            if (pairs != null && pairs.ContainsKey(key)) {
                if (int.TryParse(pairs[key], out softId)) {
                    return softId;
                }
            }
            return null;
        }

        public static int DefineValueAttrributeAsInt(Dictionary<string, object>? attrs, string key) {
            if (attrs != null) {
                var data = attrs.GetValueOrDefault(key);
                if (data != null) {
                    if (data is string) {
                        int outdata;
                        if (int.TryParse((string)data, out outdata))
                            return outdata;
                    }
                    if (data is string[]) {
                        int outdata;
                        if (int.TryParse((data as string[])[0], out outdata))
                            return outdata;
                    }
                }
            }
            return 0;
        }

        public static string DefineValueAttrributeAsString(Dictionary<string, object>? attrs, string key) {
            if (attrs != null) {
                var data = attrs.GetValueOrDefault(key);
                if (data != null) {
                    if (data is string)
                        return (string)data;
                    if (data is string[])
                        return (data as string[])[0];
                }
            }
            return "Неопределенно";
        }

        public static List<string> DefineValueAttrributeAsStringArray(Dictionary<string, object>? attrs, string key) {
            if (attrs != null) {
                var data = attrs.GetValueOrDefault(key);
                if (data != null) {
                    if (data is string) {
                        var arr = (string)data;
                        return arr.Split(",").ToList();
                    }
                    if (data is string[])
                        return (data as string[]).ToList();
                }
            }
            return new List<string>() { "Неопределенно" };
        }

        public static bool DefineValueAttrributeAsBool(Dictionary<string, object>? attrs, string key) {
            if (attrs != null) {
                var data = attrs.GetValueOrDefault(key);
                if (data != null && data is string) {
                    if ((data as string) == "true")
                        return true;
                }
                if (data != null && data is string[]) {
                    if ((data as string[])[0] == "true")
                        return true;
                }
            }
            return false;
        }
    }
}
