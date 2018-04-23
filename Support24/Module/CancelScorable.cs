namespace Support24
{
    internal class CancelScorable
    {
        private IDialogTask dialogTask;

        public CancelScorable(IDialogTask dialogTask)
        {
            this.dialogTask = dialogTask;
        }
    }
}