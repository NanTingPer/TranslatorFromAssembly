提取，编辑，然后导出

目前已经完成提取编辑导出

- 项目使用[AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)
- 提取部分使用[Mono.Cecil](https://github.com/jbevain/cecil)
- 文件保存使用[SQLite-net](https://github.com/praeclarum/sqlite-net)
- 汉化挂钩使用源码[[TerrariaTigerForceLocalizationLib](https://github.com/TigerChenzzz/TerrariaTigerForceLocalizationLib)
  - 文件位于源码目录 : TranslatorFromAssembly/TranslatorFromAssembly/TranslatorLibrary/ForceLocalizeSystem

## 特别鸣谢

- [Tigerzzz](https://github.com/TigerChenzzz) 提供技术支持
- [凌](https://github.com/NLick47) 提供技术指导
- [渝北](https://github.com/zlzhaidou) 提供精神支持
- 小西王 [月渎](https://github.com/moonditch)帮助测试



## 使用

#### 1. 内容提取

- 打开软件

- 在文本框中输入`dll`的全路径

- 单击提取

  ![image-20250508214708700](./markdownImg/image-20250508214708700.png)

- 随后单击`打开资源文件夹`，并进入`ILHjson`文件夹

  ![image-20250508214817487](./markdownImg/image-20250508214817487.png)

- 找到已模组名称开头的`hjson`文件并进行编辑

  ![image-20250508214855214](./markdownImg/image-20250508214855214.png)

  ![image-20250508214924870](./markdownImg/image-20250508214924870.png)

- 然后回到软件中，单击`导出硬编码`

  ![image-20250508215011349](./markdownImg/image-20250508215011349.png)

- 左侧选中要被汉化的模组，并按要求填写后，单击导出

  ![image-20250508215054286](./markdownImg/image-20250508215054286.png)
