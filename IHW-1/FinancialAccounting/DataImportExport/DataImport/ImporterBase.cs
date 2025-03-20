using System;
using System.IO;
using System.Threading.Tasks;

namespace FinancialAccounting.DataImportExport.DataImport
{
    public abstract class ImporterBase
    {
        public virtual async Task Import(string path)
        {
            var content = await File.ReadAllTextAsync(path);
            Parse(content);
        }

        protected abstract void Parse(string content);
    }
}
