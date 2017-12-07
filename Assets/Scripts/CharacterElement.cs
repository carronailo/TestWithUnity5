using System;
using System.Text;

// 用于存储特定角色资源的管理模块，包含用于作为查找索引的模型名称，部件分类，贴图序号，
// 一个CharacterElement只包含一个特定的材质球相关资源，也就是由模型，部件和贴图序号定位到的唯一资源
// 变更：elementsName变为数组，主要用于当相同模型，相同部位，相同贴图序号下，需要使用多个材质球的时候
// （比如，将原本的头部位里头发和脸拆分为两个部分，那么模型里头这个部位的renerer就需要加载两个材质球，一个是头，一个是脸，但他们还是同一个模型同一个部位同一个贴图id序列）
[Serializable]
public class CharacterElement
{
	public string modelName;
	public string partName;
	public int textureIndex;
	public string[] elementsName;
	public string bundleName;
	public string bundleFileName;
	public bool encrypt;
	public bool useInternal = false;

	public CharacterElement(string modelName, string partName, string elementName, int textureIndex, bool encrypt)
	{
		this.modelName = modelName;
		this.partName = partName;
		this.textureIndex = textureIndex;
		elementsName = new string[] { string.Format("{0}-{1}-{2:d2}", modelName, elementName, textureIndex) };
		bundleName = string.Format("{0}-{1}", modelName, partName);
		this.encrypt = encrypt;
		if (encrypt)
			bundleFileName = bundleName + ".S.bytes";
		else
			bundleFileName = bundleName + ".bytes";
	}

	public void AddElementName(string elementName)
	{
		Array.Resize(ref elementsName, elementsName.Length + 1);
		elementsName[elementsName.Length - 1] = string.Format("{0}-{1}-{2:d2}", modelName, elementName, textureIndex);
	}

	public override bool Equals(object obj)
	{
		if (obj is CharacterElement)
		{
			CharacterElement ce = obj as CharacterElement;
			if (ce.elementsName.Length != elementsName.Length)
				return false;
			for(int i = 0; i < elementsName.Length; ++i)
			{
				if (!ce.elementsName[i].Equals(elementsName[i]))
					return false;
			}
			return true;
		}
		return false;
	}

	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < elementsName.Length; ++i)
			sb.Append(elementsName[i]);
		return sb.ToString();
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

}
