// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GUI.TestRunnerView;
using Folder = DataDictionary.Tests.Translations.Folder;
using SourceText = DataDictionary.Tests.Translations.SourceText;
using Step = DataDictionary.Tests.Step;
using Translation = DataDictionary.Tests.Translations.Translation;
using TranslationDictionary = DataDictionary.Tests.Translations.TranslationDictionary;

namespace GUI.TranslationRules
{
    public class TranslationDictionaryTreeNode : ModelElementTreeNode<TranslationDictionary>
    {
        private class ItemEditor : NamedEditor
        {
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="item"></param>
        /// <param name="buildSubNodes"></param>
        public TranslationDictionaryTreeNode(TranslationDictionary item, bool buildSubNodes)
            : base(item, buildSubNodes, "Dictionary", true)
        {
        }

        /// <summary>
        ///     Builds the subnodes of this node
        /// </summary>
        /// <param name="subNodes"></param>
        /// <param name="recursive">Indicates whether the subnodes of the nodes should also be built</param>
        public override void BuildSubNodes(List<BaseTreeNode> subNodes, bool recursive)
        {
            base.BuildSubNodes(subNodes, recursive);

            foreach (Folder folder in Item.Folders)
            {
                subNodes.Add(new FolderTreeNode(folder, recursive));
            }
            foreach (Translation translation in Item.Translations)
            {
                subNodes.Add(new TranslationTreeNode(translation, recursive));
            }
            subNodes.Sort();
        }

        /// <summary>
        ///     Creates the editor for this tree node
        /// </summary>
        /// <returns></returns>
        protected override Editor CreateEditor()
        {
            return new ItemEditor();
        }

        public void AddFolderHandler(object sender, EventArgs args)
        {
            Item.appendFolders(Folder.CreateDefault(Item.Folders));
        }

        /// <summary>
        ///     Creates a new translation
        /// </summary>
        /// <param name="translation"></param>
        /// <returns></returns>
        public void CreateTranslation(Translation translation)
        {
            Translation existingTranslation = null;
            foreach (SourceText sourceText in translation.SourceTexts)
            {
                existingTranslation = Item.FindExistingTranslation(sourceText);
                if (existingTranslation != null)
                {
                    break;
                }
            }

            if (existingTranslation != null)
            {
                DialogResult dialogResult =
                    MessageBox.Show(
                        @"Translation already exists. Do you want to create a new one (Cancel will select the existing translation) ?",
                        @"Already existing translation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (dialogResult == DialogResult.OK)
                {
                    existingTranslation = null;
                }
            }

            if (existingTranslation == null)
            {
                Item.appendTranslations(translation);
            }
        }

        public void AddTranslationHandler(object sender, EventArgs args)
        {
            CreateTranslation(Translation.CreateDefault(Item.Translations, null));
        }

        /// <summary>
        ///     The menu items for this tree node
        /// </summary>
        /// <returns></returns>
        protected override List<MenuItem> GetMenuItems()
        {
            List<MenuItem> retVal = new List<MenuItem>
            {
                new MenuItem("Add folder", AddFolderHandler),
                new MenuItem("Add translation", AddTranslationHandler)
            };

            return retVal;
        }

        /// <summary>
        ///     Creates a new translation based on a step
        /// </summary>
        /// <param name="step"></param>
        private void CreateTranslation(Step step)
        {
            CreateTranslation(Translation.CreateDefault(Item.Translations, step.CreateSourceText()));
        }

        /// <summary>
        ///     Accepts a drop event
        /// </summary>
        /// <param name="sourceNode"></param>
        public override void AcceptDrop(BaseTreeNode sourceNode)
        {
            base.AcceptDrop(sourceNode);
            if (sourceNode is StepTreeNode)
            {
                StepTreeNode step = sourceNode as StepTreeNode;

                CreateTranslation(step.Item);
            }
            else if (sourceNode is TranslationTreeNode)
            {
                TranslationTreeNode translation = sourceNode as TranslationTreeNode;
                Translation otherTranslation = translation.Item;
                translation.Delete();
                CreateTranslation(otherTranslation);
            }
        }
    }
}