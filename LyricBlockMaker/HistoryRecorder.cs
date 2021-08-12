using System.Windows.Controls;

namespace LyricBlockMaker
{
    //<summary>
    //历史记录器
    //<summary>
    public class HistoryRecorder
    {
        private ListBox list;//to record the changing description
        private int position;//to show where it is now
        private string description;//to describe what is the change
        private Grid grid;//the object need to be recorded
        public HistoryRecorder()
        {
            list = new ListBox();
        }
        public void setGrid(Grid gridView)
        {
            grid = gridView;
        }
        public void Record()
        {

        }
        public void Undo()
        {

        }
        public void Redo()
        {

        }
    }
}
