﻿using System;
using Avalonia.Controls.Models.TreeDataGrid;

namespace Avalonia.Controls.Primitives
{
    public class TreeDataGridElementFactory : IElementFactory
    {
        private readonly RecyclePool _recyclePool = new();

        public IControl Build(object? data)
        {
            var result = GetElement(data, null);
            result.DataContext = data;
            return result;
        }

        public IControl GetElement(ElementFactoryGetArgs args) => GetElement(args.Data, args.Parent);

        public bool Match(object? data) => data is ICell;

        public void RecycleElement(ElementFactoryRecycleArgs args)
        {
            if (args.Element is not null)
            {
                _recyclePool.PutElement(args.Element, GetElementRecycleKey(args.Element), args.Parent);
            }
        }

        protected virtual IControl CreateElement(object? data)
        {
            return data switch
            {
                CheckBoxCell => new TreeDataGridCheckBoxCell(),
                TemplateCell => new TreeDataGridTemplateCell(),
                IExpanderCell => new TreeDataGridExpanderCell(),
                ICell => new TreeDataGridTextCell(),
                IColumn => new TreeDataGridColumnHeader(),
                IRow => new TreeDataGridRow(),
                _ => throw new NotSupportedException(),
            };
        }

        protected virtual string GetDataRecycleKey(object? data)
        {
            return data switch
            {
                CheckBoxCell => typeof(TreeDataGridCheckBoxCell).FullName!,
                TemplateCell => typeof(TreeDataGridTemplateCell).FullName!,
                IExpanderCell => typeof(TreeDataGridExpanderCell).FullName!,
                ICell => typeof(TreeDataGridTextCell).FullName!,
                IColumn => typeof(TreeDataGridColumnHeader).FullName!,
                IRow => typeof(TreeDataGridRow).FullName!,
                _ => throw new NotSupportedException(),
            };
        }

        protected virtual string GetElementRecycleKey(IControl element)
        {
            return element.GetType().FullName!;
        }

        private IControl GetElement(object? data, IControl? parent)
        {
            var recycleKey = GetDataRecycleKey(data);

            if (_recyclePool.TryGetElement(recycleKey, parent) is IControl element)
            {
                return element;
            }

            return CreateElement(data);
        }
    }
}
