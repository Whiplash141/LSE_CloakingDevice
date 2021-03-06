﻿/*
Copyright © 2016 Leto
This work is free. You can redistribute it and/or modify it under the
terms of the Do What The Fuck You Want To Public License, Version 2,
as published by Sam Hocevar. See http://www.wtfpl.net/ for more details.
*/

using System.Collections.Generic;
using System.Text;

using Sandbox.ModAPI;
using VRage.ObjectBuilders;
using Sandbox.ModAPI.Interfaces.Terminal;
using VRage.ModAPI;

namespace LSE.Control
{
    public class ComboboxControl<T, T2> : BaseControl<T>
    {
        public static Dictionary<string, Dictionary<IMyTerminalBlock, List<MyTerminalControlListBoxItem>>> Values =
            new Dictionary<string, Dictionary<IMyTerminalBlock, List<MyTerminalControlListBoxItem>>>();
        public int Size;
        public bool Multiselect;

		public ComboboxControl(
            IMyTerminalBlock block,
			string internalName,
			string title,            
            int size = 3,
            bool multiselect = false,
			List<MyTerminalControlListBoxItem> Content = null)
            : base(block, internalName, title)
		{
            Size = size;
            Multiselect = multiselect;

			if (!Values.ContainsKey (InternalName))
			{
                Values[InternalName] = new Dictionary<IMyTerminalBlock, List<MyTerminalControlListBoxItem>>();
			}

            if (Content == null)
            {
                Content = new List<MyTerminalControlListBoxItem>();
            }

            Values[InternalName][block] = Content;
            CreateUI();
        }

        public override void OnCreateUI()
        {
			var combobox = MyAPIGateway.TerminalControls.CreateControl<IMyTerminalControlListbox, T>(InternalName);
			combobox.Visible = ShowControl;
			combobox.ListContent = FillContent;
			combobox.ItemSelected = Setter;
            combobox.VisibleRowsCount = Size;
            combobox.Enabled = Enabled;
			combobox.Title = VRage.Utils.MyStringId.GetOrCompute(Title);
            combobox.Multiselect = Multiselect;
			MyAPIGateway.TerminalControls.AddControl<T>(combobox);
        }

        public virtual void FillContent(IMyTerminalBlock block, List<MyTerminalControlListBoxItem> items, List<MyTerminalControlListBoxItem> selected)
		{


            items.Clear();
            selected.Clear();
            foreach (var value in Values[InternalName][block])
			{
				items.Add (value);
			}

            var indices = Getter(block);
            foreach (var index in indices)
            {
                selected.Add(items[(int)index]);
            }

            if (selected.Count == 0 && items.Count > 0)
            {
                selected.Add(items[0]);
                Setter(block, selected);
            }
		}

        /*
		public virtual List<long> Getter(IMyTerminalBlock block)
        {
            var names = new List<string>();
            var indices = new List<long>();
            MyAPIGateway.Utilities.GetVariable<List<string>>(block.EntityId.ToString() + InternalName, out names);
            if (names != null)
            {
                foreach (var name in names)
                {
                    var index = Values[InternalName][block].IndexOf(name);
                    if (index != -1)
                    {
                        indices.Add(index);
                    }
                }
            }
            return indices;
        }
        */



        public virtual List<string> GetterName(IMyTerminalBlock block)
        {

            var names = new List<string>();
            var indices = this.Getter(block);
            foreach (var index in indices)
            {
                if (index < Values[InternalName][block].Count)
                {
                    names.Add(Values[InternalName][block][index].Text.String);
                }
            }

            if (names == null)
            {
                return new List<string>();
            }
            return names;
        }

        public virtual List<T2> GetterObjects(IMyTerminalBlock block)
        {
            var names = new List<T2>();
            var indices = Getter(block);
            foreach (var index in indices)
            {
                if (index < Values[InternalName][block].Count)
                {
                    names.Add((T2)Values[InternalName][block][index].UserData);
                }
            }

            if (names == null)
            {
                return new List<T2>();
            }
            return names;
        }

        public virtual List<int> Getter(IMyTerminalBlock block)
        {
            var indices = new List<int>();
            var objects = new List<T2>();
            MyAPIGateway.Utilities.GetVariable<List<T2>>(block.EntityId.ToString() + InternalName, out objects);
            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    var index = Values[InternalName][block].FindIndex((x) => obj.Equals(x.UserData));
                    if (index >= 0)
                    {
                        indices.Add(index);
                    }
                }
            }

            return indices;
        }

        public virtual void Setter(IMyTerminalBlock block, List<MyTerminalControlListBoxItem> selected)
        {
            
            var objects = new List<T2>();
            foreach (var item in selected)
            {
                try
                {
                    objects.Add((T2)item.UserData);
                }
                catch
                {
                }
            }
            MyAPIGateway.Utilities.SetVariable<List<T2>>(block.EntityId.ToString() + InternalName, objects);
        }
    }
}