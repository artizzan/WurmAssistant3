using Ninject;

namespace AldursLab.WurmAssistant3.Systems.ConventionBinding
{
    /// <summary>
    /// This interface can be used to mark a single public class within an Area.
    /// It can be used to define custom binding logic for this Area.
    /// </summary>
    public interface IAreaConfiguration
    {
        /// <summary>
        /// Implements custom Kernel configuration for this area.
        /// Note, that execution order between Areas is by default arbitrary, unless explicitly staged at the Core.
        /// </summary>
        /// <param name="kernel"></param>
        void Configure(IKernel kernel);
    }
}