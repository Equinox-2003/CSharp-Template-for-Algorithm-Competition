using System.Text;
using TemplateA;

[TestClass]
public class ProblemTest
{
    [TestMethod]
    [DataRow("""
    4 7
    1 0 1
    0 0 1
    0 2 3
    1 0 1
    1 1 2
    0 0 2
    1 1 3
    
    """, """
    0
    1
    0
    1
    
    """)]
    public void TestMethod1(string input, string expected)
    {
        // 输入转化成流
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        using var inputStream = new MemoryStream(inputBytes);
        using var outputStream = new MemoryStream();

        var snippet = new SolutionA(inputStream, outputStream);
        snippet._Main();

        outputStream.Position = 0;
        using var reader = new StreamReader(outputStream);
        string actual = reader.ReadToEnd();

        // 将实际输出和预期输出中的 \r\n 全部替换为 \n
        // 并且 Trim() 掉末尾可能的空行
        string normalizedActual = actual.Replace("\r\n", "\n").Trim();
        string normalizedExpected = expected.Replace("\r\n", "\n").Trim();

        Assert.AreEqual(normalizedExpected, normalizedActual);
    }
}