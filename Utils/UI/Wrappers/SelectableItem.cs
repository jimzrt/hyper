using System;

namespace Utils.UI.Wrappers
{
    public class SelectableItem<T> : EntityBase
    {
        public Action OnSelectedCallback { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                if (OnSelectedCallback != null && value)
                    OnSelectedCallback();
                Notify("IsSelected");
            }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                Notify("IsEnabled");
            }
        }

        private T _item;
        public T Item
        {
            get { return _item; }
            set
            {
                _item = value;
                Notify("Item");
            }
        }

        public SelectableItem(T item)
        {
            Item = item;
        }

        public override string ToString()
        {
            var obj = Item as object;
            if (obj != null)
                return obj.ToString();

            return base.ToString();
        }

        public void RefreshBinding()
        {
            Notify("Item");
        }
    }

    public class SelectableItemWithComment<T> : SelectableItem<T>
    {
        public SelectableItemWithComment()
            : base(default(T))
        {

        }

        public SelectableItemWithComment(T item)
            : base(item)
        {

        }

        private string _mComment = "";
        public string Comment
        {
            get { return _mComment; }
            set
            {
                _mComment = value;
                Notify("Comment");
            }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Comment))
            {
                return base.ToString();
            }
            return string.Format("{0} ({1})", base.ToString(), Comment);
        }
    }
}
