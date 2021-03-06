﻿using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace NUnit.ProjectEditor.Tests.Presenters
{
    public class RenameConfigurationPresenterTests
    {
        IPropertyModel model;
        IRenameConfigurationDialog dlg;
        RenameConfigurationPresenter presenter;

        [SetUp]
        public void Initialize()
        {
            var doc = new ProjectModel();
            doc.LoadXml(NUnitProjectXml.EmptyConfigs);
            model = new PropertyModel(doc);
            dlg = Substitute.For<IRenameConfigurationDialog>();
            presenter = new RenameConfigurationPresenter(model, dlg, "Debug");
        }

        [Test]
        public void ConfigurationName_OnLoad_IsSetToOriginalName()
        {
            Assert.AreEqual("Debug", dlg.ConfigurationName.Text);
        }

        [Test]
        public void ConfigurationName_OnLoad_OriginalNameIsSelected()
        {
            dlg.ConfigurationName.Received().Select(0,5);
        }

        [Test]
        public void OkButton_OnLoad_IsDisabled()
        {
            Assert.False(dlg.OkButton.Enabled);
        }

        [Test]
        public void ConfigurationName_WhenSetToNewName_OkButtonIsEnabled()
        {
            dlg.ConfigurationName.Text = "New";
            dlg.ConfigurationName.Changed += Raise.Event<ActionHandler>();

            Assert.True(dlg.OkButton.Enabled);
        }

        [Test]
        public void ConfigurationName_WhenSetToOriginalName_OkButtonIsDisabled()
        {
            dlg.ConfigurationName.Text = "Debug";
            dlg.ConfigurationName.Changed += Raise.Event<ActionHandler>();

            Assert.False(dlg.OkButton.Enabled);
        }

        [Test]
        public void ConfigurationName_WhenCleared_OkButtonIsDisabled()
        {
            dlg.ConfigurationName.Text = string.Empty;
            dlg.ConfigurationName.Changed += Raise.Event<ActionHandler>();

            Assert.False(dlg.OkButton.Enabled);
        }

        [Test]
        public void OkButton_WhenClicked_PerformsRename()
        {
            dlg.ConfigurationName.Text = "New";
            dlg.OkButton.Execute += Raise.Event<CommandHandler>();

            Assert.That(model.ConfigNames, Is.EqualTo(new[] { "New", "Release" }));
        }

        [Test]
        public void OkButton_RenameActiveConfig_ChangesActiveConfig()
        {
            dlg.ConfigurationName.Text = "New";
            dlg.OkButton.Execute += Raise.Event<CommandHandler>();

            Assert.That(model.ActiveConfigName, Is.EqualTo("New"));
        }

        [Test]
        public void Dialog_WhenClosedWithoutClickingOK_LeavesConfigsUnchanged()
        {
            dlg.ConfigurationName.Text = "New";
            dlg.Close();

            Assert.That(model.ConfigNames, Is.EqualTo(new[] { "Debug", "Release" }));
        }
    }
}
