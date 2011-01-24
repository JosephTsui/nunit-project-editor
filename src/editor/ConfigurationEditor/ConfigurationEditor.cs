﻿// ***********************************************************************
// Copyright (c) 2010 Charlie Poole
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ***********************************************************************

using System;

namespace NUnit.ProjectEditor
{
    public class ConfigurationEditor
    {
        #region Instance Variables

        private IProjectModel model;
        private IConfigurationEditorView view;

        #endregion

        #region Constructor

        public ConfigurationEditor(IProjectModel model, IConfigurationEditorView view)
        {
            this.model = model;
            this.view = view;

            UpdateConfigList();

            view.AddCommand.Execute += AddConfig;
            view.RemoveCommand.Execute += RemoveConfig;
            view.RenameCommand.Execute += RenameConfig;
            view.ActiveCommand.Execute += MakeActive;

            view.ConfigList.SelectionChanged += SelectedConfigChanged;
        }

        #endregion

        #region Command Event Handlers

        public void AddConfig()
        {
            AddConfigData data = new AddConfigData();
            if (view.GetAddConfigData(ref data))
            {
                model.Configs.Add(data.ConfigToCreate);
                IProjectConfig newConfig = model.Configs[data.ConfigToCreate];

                if (data.ConfigToCopy != null)
                {
                    IProjectConfig copyConfig = model.Configs[data.ConfigToCopy];
                    if (copyConfig != null)
                    {
                        newConfig.BasePath = copyConfig.BasePath;
                        newConfig.BinPathType = copyConfig.BinPathType;
                        if (newConfig.BinPathType == BinPathType.Manual)
                            newConfig.PrivateBinPath = copyConfig.PrivateBinPath;
                        newConfig.ConfigurationFile = copyConfig.ConfigurationFile;
                        newConfig.RuntimeFramework = copyConfig.RuntimeFramework;

                        foreach (string assembly in copyConfig.Assemblies)
                            newConfig.Assemblies.Add(assembly);
                    }
                }

                UpdateConfigList();
            }
        }

        public void RenameConfig()
        {
            string oldName = view.ConfigList.SelectedItem;

            string newName = view.GetNewNameForRename(oldName);

            if (newName != null)
            {
                model.Configs[oldName].Name = newName;
                UpdateConfigList();
            }
        }

        public void RemoveConfig()
        {
            model.Configs.RemoveAt(view.ConfigList.SelectedIndex);

            UpdateConfigList();
        }

        public void MakeActive()
        {
            model.ActiveConfigName = view.ConfigList.SelectedItem;

            UpdateConfigList();
        }

        public void SelectedConfigChanged()
        {
            int index = view.ConfigList.SelectedIndex;

            view.AddCommand.Enabled = true;
            view.ActiveCommand.Enabled = index >= 0 && model.Configs[index].Name != model.ActiveConfigName;
            view.RenameCommand.Enabled = index >= 0;
            view.RemoveCommand.Enabled = index >= 0;
        }

        #endregion

        #region Helper Methods

        private void UpdateConfigList()
        {
            string selectedConfig = view.ConfigList.SelectedItem;
            if (selectedConfig != null && selectedConfig.EndsWith(" (active)"))
                selectedConfig = selectedConfig.Substring(0, selectedConfig.Length - 9);
            int selectedIndex = -1;
            int activeIndex = -1;

            int count = model.Configs.Count;
            string[] configList = new string[count];

            for (int index = 0; index < count; index++)
            {
                string config = model.Configs[index].Name;

                if (config == model.ActiveConfigName)
                    activeIndex = index;
                if (config == selectedConfig)
                    selectedIndex = index;

                configList[index] = config;
            }

            if (activeIndex >= 0)
                configList[activeIndex] += " (active)";

            view.ConfigList.SelectionList = configList;

            view.ConfigList.SelectedIndex = selectedIndex > 0
                ? selectedIndex
                : configList.Length > 0
                    ? 0 : -1;

            SelectedConfigChanged();
        }

        #endregion
    }
}