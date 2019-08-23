using System.Collections;

namespace Utils.UI.Wrappers
{
    public class FolderItem
    {
        public string Name { get; set; }
        public IEnumerable Items { get; set; }
        public object Parent { get; set; }
    }
}
