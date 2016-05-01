using System;

namespace AldursLab.WurmAssistant3.Core.Areas.Wa2DataImport.Modules
{
    public abstract class ImportItem
    {
        public MergeResult MergeResult { get; set; }

        public abstract string SourceAspect { get; }
        public abstract string DestinationAspect { get; }

        public string ImportAsNewAspect
        {
            get { return (Blocked || HasDestination) ? null : "Import"; }
        }

        public string DoNotImportAspect
        {
            get { return null; }
        }

        public string Comment { get; set; }
        public string CommentAspect { get { return Comment ?? string.Empty; } }

        public bool Blocked { get; set; }

        public abstract void Resolve();

        public override string ToString()
        {
            return SourceAspect + " > " + DestinationAspect;
        }

        public event EventHandler<EventArgs> Resolved;

        protected virtual void OnResolved()
        {
            var handler = Resolved;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public abstract bool HasDestination { get; }
    }

    public class ImportItem<TSource, TDestination> : ImportItem
    {
        public TSource Source { get; set; }
        public TDestination Destination { get; set; }
        public Action<MergeResult, TSource, TDestination> ResolutionAction { get; set; }
        public Func<TSource, string> SourceAspectConverter { get; set; }
        public Func<TDestination, string> DestinationAspectConverter { get; set; }

        public override string SourceAspect
        {
            get
            {
                if (Source != null)
                {
                    if (SourceAspectConverter != null)
                    {
                        return SourceAspectConverter(Source);
                    }
                    return Source.ToString();
                }
                return "";
            }
        }

        public override string DestinationAspect
        {
            get
            {
                if (Destination != null)
                {
                    if (DestinationAspectConverter != null)
                    {
                        return DestinationAspectConverter(Destination);
                    }
                    return Destination.ToString();
                }
                return "";
            }
        }

        public override void Resolve()
        {
            if (ResolutionAction != null)
            {
                ResolutionAction(MergeResult, Source, Destination);
            }
            OnResolved();
        }

        public override bool HasDestination
        {
            get { return Destination != null; }
        }
    }
}
