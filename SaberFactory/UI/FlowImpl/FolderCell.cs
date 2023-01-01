namespace SaberFactory.UI.Flow;

public partial class FolderCell
{
    private IFolderInfo _cellData;
    
    public override void SetData(object data)
    {
        _cellData = (IFolderInfo)data;

        folderTextmesh.text = _cellData.Name;
    }
}