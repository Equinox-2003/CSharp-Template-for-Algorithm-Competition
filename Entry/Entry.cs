using System.Reflection;

Console.Write("输入题目编号 (如 B, C): ");
string prob = Console.ReadLine()?.ToUpper();
if (string.IsNullOrEmpty(prob)) return;

string name = $"Template{prob}.Solution{prob}";
Type type = Type.GetType(name);

if (type == null) {
    // 如果找不到类，说明是第一次做这题，从 Template.cs 生成
    // 这里根据实际编译路径调整，通常 bin/Debug/net8.0 需要向上跳 3 级回到源码目录
    string projectDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../");
    string templatePath = Path.Combine(projectDir, "Template.cs");
    string targetPath = Path.Combine(projectDir, $"{prob}.cs");

    if (File.Exists(templatePath)) {
        string content = File.ReadAllText(templatePath);
        // 替换命名空间和类名
        content = content.Replace("Template", $"Template{prob}")
                         .Replace("Solution", $"Solution{prob}");

        File.WriteAllText(targetPath, content);
        Console.WriteLine($"已生成文件: {prob}.cs。请在 VS 中点击“保存所有文件”并【重新生成项目】后再运行。");
    } else {
        Console.WriteLine("错误：找不到模板文件 A.cs");
    }
} else {
    // 如果找到了类，直接运行
    object sol = Activator.CreateInstance(type);
    MethodInfo solve = type.GetMethod("Main");
    if (solve != null) {
        Console.WriteLine($"--- 正在运行题目 {prob} ---");
        solve.Invoke(sol, null);
    }
}