提取，编辑，然后导出

目前已经完成提取编辑导出

- 项目使用[AvaloniaUI](https://github.com/AvaloniaUI/Avalonia)
- 提取部分使用[Mono.Cecil](https://github.com/jbevain/cecil)
- 文件保存使用[SQLite-net](https://github.com/praeclarum/sqlite-net)
- 汉化挂钩使用源码[[TerrariaTigerForceLocalizationLib](https://github.com/TigerChenzzz/TerrariaTigerForceLocalizationLib)
  - 文件位于源码目录 : TranslatorFromAssembly/TranslatorFromAssembly/TranslatorLibrary/ForceLocalizeSystem




- 本项目为小型项目，也是我的第一个自己独立动手完成的项目



汉化讨论 / 反馈(但愿) : 909428503 (神秘企鹅数字)

## 特别鸣谢

- [Tigerzzz](https://github.com/TigerChenzzz) 提供技术支持
- [凌](https://github.com/NLick47) 提供技术指导
- [渝北](https://github.com/zlzhaidou) 提供精神支持
- 小西王 帮助测试



## 使用

> #### 过滤条件不能过滤出正确的内容 需要使用者自行分辨
>
> ##### 软件提供不显示功能 可以将被选中项目设置为不显示 帮助使用者进行数据过滤

1. 打开软件
2. 将一开始的文本 替换为需要被汉化的模组的dll文件目录

> 由于使用异步编程，提取不会影响UI线程 但用户的行为会干扰后续的任务

3. 点击确定 等待一会 等到下方表格出现内容(也可能任然还在加载)
4. 单机左上角的打开 选中从`根据本地`
5. 即便打开`根据本地`后有内容也是不行的请继续按照下面的步骤
6. 单机左上角`类名`左边的`三` (真的是中文的三)
7. 单机需要被汉化的`模组`

> 即便你编辑了其他列，也是不会生效更改的

8. 随后可以直接在下方表格中进行编辑 , 编辑`中文`列 



> #### 导出

9. 单机左上角`打开` 按钮 单机`导出`选项
10. 在左边单机需要被导出的项目
11. 在右边填入对应的参数
    1. 你的模组名称 => 你自己的模组名称
    2. 目标模组名称 => 要被汉化的模组名称
    3. 你的模组所在目录 => 你自己的模组的根目录
12. 单机`导出`
13. 打开你的模组所在目录下的 `Systems`目录
14. 打开里面的文件(应该只有一个)
15. 编辑名称空间`namespace FargoCP.Systems;` 改为`你的模组名称.Systems`



## 无法过滤项

> ### 1

```csharp
DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
defaultInterpolatedStringHandler.AppendLiteral("randrsd");
defaultInterpolatedStringHandler.AppendFormatted<int>(l);
Gore.NewGore(source_FromThis, vector, vector2, ModContent.Find<ModGore>(name, defaultInterpolatedStringHandler.ToStringAndClear()).Type, base.NPC.scale);
```

> ### 2

```csharp
ldstr     "/Assets/Sounds/BaronHit"
ldc.i4.0
newobj    instance void [tModLoader]Terraria.Audio.SoundStyle::.ctor(string, valuetype [tModLoader]Terraria.Audio.SoundType)
```

> ### 3

```csharp
public override LocalizedText DefaultContainerName(int frameX, int frameY)
{
	return ILocalizedModTypeExtensions.GetLocalization(this, "MapEntry" + (frameX / 36).ToString(), null);
}
```

> 以及众多LocalizedText
