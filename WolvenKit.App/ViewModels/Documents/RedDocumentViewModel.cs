using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using WolvenKit.Common;
using WolvenKit.Common.FNV1A;
using WolvenKit.Common.Model;
using WolvenKit.Common.Model.Cr2w;
using WolvenKit.Common.Services;
using WolvenKit.Functionality.Commands;
using WolvenKit.Functionality.Controllers;
using WolvenKit.Functionality.Services;
using WolvenKit.Models.Editor;
using WolvenKit.MVVM.Model.ProjectManagement.Project;
using WolvenKit.RED4.CR2W;
using WolvenKit.ViewModels.Shell;

namespace WolvenKit.ViewModels.Documents
{
    public class RedDocumentViewModel : DocumentViewModel
    {
        private readonly IGameControllerFactory _gameControllerFactory;
        private readonly IProjectManager _projectManager;
        private readonly ILoggerService _loggerService;
        private readonly Red4ParserService _wolvenkitFileService;
        private readonly IArchiveManager _archiveManager;


        public RedDocumentViewModel(string path) : base(path)
        {
            _loggerService = Locator.Current.GetService<ILoggerService>();
            _gameControllerFactory = Locator.Current.GetService<IGameControllerFactory>();
            _projectManager = Locator.Current.GetService<IProjectManager>();
            _wolvenkitFileService = Locator.Current.GetService<Red4ParserService>();
            _archiveManager = Locator.Current.GetService<IArchiveManager>();


            OpenEditorCommand = new RelayCommand(ExecuteOpenEditor);
            OpenBufferCommand = new RelayCommand(ExecuteOpenBuffer);
            OpenImportCommand = new DelegateCommand<ICR2WImport>(ExecuteOpenImport);

            ViewChunksCommand = new RelayCommand(ExecuteViewChunks, CanViewChunks);
            ViewImportsCommand = new RelayCommand(ExecuteViewImports, CanViewImports);
            ViewBuffersCommand = new RelayCommand(ExecuteViewBuffers, CanViewBuffers);
            ViewEditorsCommand = new RelayCommand(ExecuteViewEditors, CanViewEditors);

            this.WhenAnyValue(x => x.SelectedChunk).Subscribe(chunk =>
            {
                if (chunk != null)
                {
                    ChunkProperties = new ObservableCollection<ChunkPropertyViewModel>(
                        SelectedChunk.GetData()
                            .ChildrEditableVariables
                            .Select(x => new ChunkPropertyViewModel(x)));

                }
            });

        }

        #region commands

        public ICommand OpenBufferCommand { get; private set; }
        private bool CanOpenBuffer() => true;
        private void ExecuteOpenBuffer()
        {
            // TODO: Handle command logic here
        }

        public ICommand OpenEditorCommand { get; private set; }
        private bool CanOpenEditor() => true;
        private void ExecuteOpenEditor()
        {




        }

        public ICommand ViewChunksCommand { get; private set; }
        private bool CanViewChunks() => true;
        private void ExecuteViewChunks()
        {
            ChunksVisibility = true;
            ImportsVisibility = false;
            BuffersVisibility = false;
            EditorsVisibility = false;
        }

        public ICommand ViewImportsCommand { get; private set; }
        private bool CanViewImports() => Imports.Any();
        private void ExecuteViewImports()
        {
            ChunksVisibility = false;
            ImportsVisibility = true;
            BuffersVisibility = false;
            EditorsVisibility = false;
        }

        public ICommand ViewBuffersCommand { get; private set; }
        private bool CanViewBuffers() => Buffers.Any();
        private void ExecuteViewBuffers()
        {
            ChunksVisibility = false;
            ImportsVisibility = false;
            BuffersVisibility = true;
            EditorsVisibility = false;
        }

        public ICommand ViewEditorsCommand { get; private set; }
        private bool CanViewEditors() => Buffers.Any();
        private void ExecuteViewEditors()
        {
            ChunksVisibility = false;
            ImportsVisibility = false;
            BuffersVisibility = false;
            EditorsVisibility = true;
        }

        public ICommand OpenImportCommand { get; private set; }
        private void ExecuteOpenImport(ICR2WImport input)
        {
            var depotpath = input.DepotPathStr;
            var key = FNV1A64HashAlgorithm.HashString(depotpath);

            if (_archiveManager.Lookup(key).HasValue)
            {
                _gameControllerFactory.GetController().AddToMod(key);
            }
        }

        #endregion

        #region properties

        [Reactive] public ObservableCollection<ChunkPropertyViewModel> ChunkProperties { get; set; } = new();


        [Reactive] public IWolvenkitFile File { get; set; }

        public List<ICR2WImport> Imports => File.Imports;

        [Reactive] public ICR2WImport SelectedImport { get; set; }

        public List<ICR2WBuffer> Buffers => File.Buffers;

        [Reactive] public ICR2WBuffer SelectedBuffer { get; set; }

        public List<ChunkViewModel> Chunks => File.Chunks
            .Where(_ => _.VirtualParentChunk == null)
            .Select(_ => new ChunkViewModel(_)).ToList();

        [Reactive] public ChunkViewModel SelectedChunk { get; set; }

        public List<EEditorType> Editors => GetEditors();

        [Reactive] public EEditorType SelectedEditor { get; set; }


        [Reactive] public bool ChunksVisibility { get; set; } = true;

        [Reactive] public bool ImportsVisibility { get; set; }

        [Reactive] public bool BuffersVisibility { get; set; }

        [Reactive] public bool EditorsVisibility { get; set; }

        #endregion

        #region methods

        public override void OnSave(object parameter)
        {
            using var fs = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite);
            using var bw = new BinaryWriter(fs);
            File.Write(bw);
        }

        public override async Task<bool> OpenFileAsync(string path)
        {
            _isInitialized = false;

            try
            {
                await using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using var reader = new BinaryReader(stream);

                    var cr2w = _wolvenkitFileService.TryReadCr2WFile(reader);
                    if (cr2w == null)
                    {
                        _loggerService.Error($"Failed to read cr2w file {path}");
                        return false;
                    }
                    cr2w.FileName = path;

                    File = cr2w;

                    ContentId = path;
                    FilePath = path;
                    IsDirty = false;
                    Title = FileName;
                    _isInitialized = true;
                }

                return true;
            }
            catch (Exception e)
            {
                _loggerService.Error(e);
                // Not processing this catch in any other way than rejecting to initialize this
                _isInitialized = false;
            }

            return false;
        }

        private List<EEditorType> GetEditors()
        {
            var editors = new List<EEditorType>()
            {
                EEditorType.W2RCEditor
            };
            var extension = Path.GetExtension(FilePath);
            if (Enum.TryParse<ERedExtension>(extension, out var redExtension))
            {
                switch (redExtension)
                {
                    case ERedExtension.csv:
                        editors.Add(EEditorType.CsvEditor);
                        break;
                    default:
                        break;
                }
            }
            

            return editors;
            #endregion
        }
    }
}
