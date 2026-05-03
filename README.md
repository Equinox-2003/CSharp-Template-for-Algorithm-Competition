# CSharp-Template-for-Algorithm-Competition

## 一、IDE

**VS2026**

## 二、模板库

来自CSharp版本的atc模板库：https://github.com/kzrnm/ac-library-csharp

## 三、如何使用

### 3.1 安装 SourceExpander

在 VS2026 中打开管理NuGet包安装SourceExpander 这个包
<img width="1012" height="686" alt="image" src="https://github.com/user-attachments/assets/c036915d-3807-4c3b-a8c2-0df66f37e291" />


### 3.2 安装 ac-library-csharp

同样的，找到 ac-library-csharp 包并安装

<img width="1479" height="717" alt="image" src="https://github.com/user-attachments/assets/5ed97041-88ec-4971-965d-8dde148283bc" />


### 3.3 在本地使用atc模板库

<img width="1439" height="650" alt="image" src="https://github.com/user-attachments/assets/ead9243b-b59d-4aca-a76b-666317a1ff65" />


装完上面两个包，就可以正常使用 atc的板子了。

在主程序中加入 `            SourceExpander.Expander.Expand();`

这样运行后就会生成一个 文件`Combined.csx`， SourceExpander 会在该文件尾部帮你展开你所引用的命名空间
<img width="1334" height="250" alt="image" src="https://github.com/user-attachments/assets/ce6e3cef-817d-4566-85c4-96fc63e699b0" />


### 3.4 如何展开自定义库

#### 3.4.1 创建自定义类库

自己在VS中新建个项目，把想要添加的类定义放里面

比如我的IO板子：

<img width="1909" height="594" alt="image" src="https://github.com/user-attachments/assets/b89f81eb-9152-4119-b033-3fdf58e6e3ab" />


#### 3.4.2 安装 SourceExpander.Embedder

这次安装：**SourceExpander.Embedder**（**注意，在类库项目中安装，如果同时安装SourceExpander 和 SourceExpander.Embedder无法正常运行**）

<img width="1440" height="622" alt="image" src="https://github.com/user-attachments/assets/81d6af95-4d8b-4d70-a7a9-ee7b75537ff9" />


然后在右侧解决方案处右键项目点击生成

<img width="511" height="924" alt="image" src="https://github.com/user-attachments/assets/851a74f5-09c8-4ffb-9dda-2ac5d2d1730b" />


这样目录下面会有dll文件，回到写代码的项目，右键点击添加-项目引用

<img width="908" height="1015" alt="image" src="https://github.com/user-attachments/assets/c8fbeb6b-d3fb-4035-a8be-d57f0e3ac1c1" />


点击浏览，找到dll文件，然后就可以引用类库的内容了

### 3.5 利用反射创建模板代码文件

就是写完A题，让脚本自动帮你创建B题的文件：Templates/Entry.cs

具体可以看Entry.cs 和 Template.cs代码

### 3.6 利用VS单元测试来进行用例测试

VS2026 没有 vscode 那种CPH judge 测试插件，但是VS自带的单元测试已经很方便了

测试脚本：Templates/Test1.cs

具体使用的时候创建一个测试单元的项目，把 Test1.cs 拷贝进去即可

对于每组用例：

DataRow 第一个字符串内放输入，第二个字符串放输出

多组用例直接写多个 DataRow 即可

<img width="970" height="474" alt="image" src="https://github.com/user-attachments/assets/dd71eeaf-4750-4ecd-a8bb-545aa73e3f45" />


**效果：**

<img width="1059" height="450" alt="image" src="https://github.com/user-attachments/assets/4257d181-fbff-4380-aecb-adb552bd674a" />

<img width="1061" height="452" alt="image" src="https://github.com/user-attachments/assets/d95ff7e0-2edd-4e16-96c6-cd00b5d6f23c" />



## 四、关于本仓库

**Templates** 文件夹下是若干个人补充的模板

**Entry** 文件夹：

-   Combined.csx：程序编译后，扩展包添加所用到的命名空间后，用于提交的代码
-   Entry.cs：用于创建解题模板文件的代码
-   Equinox.dll：模板项目生成的.NET 动态链接库，用来添加到对应的项目引用，即可使用模板项目中的若干模板
-   Test1.cs：测试单元，用来测试测试用例









