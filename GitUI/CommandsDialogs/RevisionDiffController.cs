﻿using System.Collections.Generic;
using GitCommands;
using GitUIPluginInterfaces;

namespace GitUI.CommandsDialogs
{
    public interface IRevisionDiffController
    {
        bool ShouldShowMenuBlame(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuCherryPick(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuEditFile(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuResetFile(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuFileHistory(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuSaveAs(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuCopyFileName(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuShowInFileTree(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuStage(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowMenuUnstage(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowSubmoduleMenus(ContextMenuSelectionInfo selectionInfo);
        bool ShouldShowDifftoolMenus(ContextMenuSelectionInfo selectionInfo);

        bool ShouldShowMenuAB(ContextMenuDiffToolInfo selectionInfo);
        bool ShouldShowMenuALocal(ContextMenuDiffToolInfo selectionInfo);
        bool ShouldShowMenuBLocal(ContextMenuDiffToolInfo selectionInfo);
        bool ShouldShowMenuAParentLocal(ContextMenuDiffToolInfo selectionInfo);
        bool ShouldShowMenuBParentLocal(ContextMenuDiffToolInfo selectionInfo);
        bool ShouldShowMenuParents(ContextMenuDiffToolInfo selectionInfo);
    }

    public sealed class ContextMenuSelectionInfo
    {
        public ContextMenuSelectionInfo(IList<GitRevision> selectedRevisions, GitItemStatus selectedDiff, bool isAnyCombinedDiff, bool isSingleGitItemSelected, bool isCombinedDiff, bool isAnyItemSelected)
        {
            SelectedRevisions = selectedRevisions;
            SelectedDiff = selectedDiff;
            IsAnyCombinedDiff  = isAnyCombinedDiff;
            IsSingleGitItemSelected = isSingleGitItemSelected;
            IsCombinedDiff = isCombinedDiff;
            IsAnyItemSelected = isAnyItemSelected;
        }
        public IList<GitRevision> SelectedRevisions { get; }
        public GitItemStatus SelectedDiff { get; }
        public bool IsAnyCombinedDiff { get; }
        public bool IsSingleGitItemSelected { get; }
        public bool IsCombinedDiff { get; }
        public bool IsAnyItemSelected { get;  }
    }

    public sealed class ContextMenuDiffToolInfo
    {
        public ContextMenuDiffToolInfo(bool aIsLocal, bool bIsLocal, bool bIsNormal, bool localExists, bool showParentItems)
        {
            AIsLocal = aIsLocal;
            BIsLocal = bIsLocal;
            BIsNormal = bIsNormal;
            LocalExists = localExists;
            ShowParentItems = showParentItems;
        }
        public bool AIsLocal { get; }
        public bool BIsLocal { get; }
        public bool BIsNormal { get; }
        public bool LocalExists { get; }
        public bool ShowParentItems { get; }
    }

    public sealed class RevisionDiffController : IRevisionDiffController
    {
        private readonly IGitModule _module;

        public RevisionDiffController(IGitModule module)
        {
            _module = module;
        }

        public bool ShouldShowDifftoolMenus(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsAnyItemSelected && !selectionInfo.IsAnyCombinedDiff;
        }

        public bool ShouldShowMenuBlame(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsSingleGitItemSelected && !(selectionInfo.SelectedDiff.IsSubmodule || selectionInfo.SelectedRevisions[0].IsArtificial());
        }

        public bool ShouldShowMenuCherryPick(ContextMenuSelectionInfo selectionInfo)
        {
            return !selectionInfo.IsCombinedDiff && selectionInfo.IsSingleGitItemSelected &&
                   !(selectionInfo.SelectedDiff.IsSubmodule || selectionInfo.SelectedRevisions[0].Guid == GitRevision.UnstagedGuid ||
                     (selectionInfo.SelectedDiff.IsNew || selectionInfo.SelectedDiff.IsDeleted) && selectionInfo.SelectedRevisions[0].Guid == GitRevision.IndexGuid);
        }

        public bool ShouldShowMenuEditFile(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsSingleGitItemSelected && !selectionInfo.SelectedDiff.IsSubmodule && selectionInfo.SelectedRevisions[0].IsArtificial();
        }

        public bool ShouldShowMenuResetFile(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsAnyItemSelected && !selectionInfo.IsCombinedDiff &&
                !(selectionInfo.IsSingleGitItemSelected && (selectionInfo.SelectedDiff.IsSubmodule || selectionInfo.SelectedDiff.IsNew) && selectionInfo.SelectedRevisions[0].Guid == GitRevision.UnstagedGuid);
        }

        public bool ShouldShowMenuFileHistory(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsSingleGitItemSelected && !(selectionInfo.SelectedDiff.IsNew && selectionInfo.SelectedRevisions[0].IsArtificial());
        }

        public bool ShouldShowMenuSaveAs(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsAnyItemSelected && !selectionInfo.IsCombinedDiff && selectionInfo.IsSingleGitItemSelected && !selectionInfo.SelectedDiff.IsSubmodule;
        }

        public bool ShouldShowMenuCopyFileName(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsAnyItemSelected;
        }

        public bool ShouldShowMenuShowInFileTree(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsSingleGitItemSelected && !selectionInfo.SelectedRevisions[0].IsArtificial();
        }

        public bool ShouldShowMenuStage(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsAnyItemSelected &&
                   selectionInfo.SelectedRevisions.Count >= 1 && selectionInfo.SelectedRevisions[0].Guid == GitRevision.UnstagedGuid ||
                   selectionInfo.SelectedRevisions.Count >= 2 && selectionInfo.SelectedRevisions[1].Guid == GitRevision.UnstagedGuid;
        }

        public bool ShouldShowMenuUnstage(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsAnyItemSelected &&
                   selectionInfo.SelectedRevisions.Count >= 1 && selectionInfo.SelectedRevisions[0].Guid == GitRevision.IndexGuid ||
                   selectionInfo.SelectedRevisions.Count >= 2 && selectionInfo.SelectedRevisions[1].Guid == GitRevision.IndexGuid;
        }

        public bool ShouldShowSubmoduleMenus(ContextMenuSelectionInfo selectionInfo)
        {
            return selectionInfo.IsSingleGitItemSelected && selectionInfo.SelectedDiff.IsSubmodule && selectionInfo.SelectedRevisions[0].Guid == GitRevision.UnstagedGuid;
        }

        public bool ShouldShowMenuAB(ContextMenuDiffToolInfo selectionInfo)
        {
            return selectionInfo.BIsNormal;
        }

        public bool ShouldShowMenuALocal(ContextMenuDiffToolInfo selectionInfo)
        {
            return selectionInfo.LocalExists && !selectionInfo.AIsLocal;
        }

        public bool ShouldShowMenuBLocal(ContextMenuDiffToolInfo selectionInfo)
        {
            return selectionInfo.LocalExists && !selectionInfo.BIsLocal && selectionInfo.BIsNormal;
        }

        public bool ShouldShowMenuAParentLocal(ContextMenuDiffToolInfo selectionInfo)
        {
            return selectionInfo.LocalExists;
        }

        public bool ShouldShowMenuBParentLocal(ContextMenuDiffToolInfo selectionInfo)
        {
            return selectionInfo.LocalExists && selectionInfo.BIsNormal;
        }

        public bool ShouldShowMenuParents(ContextMenuDiffToolInfo selectionInfo)
        {
            return selectionInfo.ShowParentItems;
        }
    }
}