using System;
using System.Collections.Generic;
using System.IO;

namespace AldursLab.WurmApi.FileSystem
{
    static class DirectoryOps
    {
        public static void CopyRecursively(string sourceDirAbsolutePath, string targetDirAbsolutePath, bool overwriteFiles = false)
        {
            var copier = new DirectoryCopier();
            copier.DirectoryCopy(sourceDirAbsolutePath, targetDirAbsolutePath, overwriteFiles);
        }
    }

    /// <summary>
    /// Can be used to copy directory, with subdirectories and optionally using 
    /// whitelist or blacklist dir paths. White and blacklists ignore case.
    /// </summary>
    internal class DirectoryCopier
    {
        HashSet<string> pathWhitelist = null;
        HashSet<string> pathBlacklist = null;

        /// <summary>
        /// Copies directory, by default includes all subdirectories recursively. 
        /// </summary>
        /// <remarks>
        /// Can optionally use whitelist or blacklist dir paths.
        /// Throws exceptions on any copy errors. 
        /// Not ACID.
        /// White and blacklists ignore case.
        /// If any subdir matches full path of any blacklisted entry, that subdir and any lower-level subdirs are ignored.
        /// Whitelisted subdirectory will always be copied, regardless of the tree depth. 
        /// There is no support for black or whitelisting individual files.
        /// Top-level directory files are always copied.
        /// </remarks>
        /// <param name="sourceDirName">full path</param>
        /// <param name="destDirName">full path</param>
        /// <param name="copySubDirs">true to copy all subdirs, false to copy only top dir</param>
        /// <param name="whitelist">list of absolute dir paths to copy recursively. Exclusive with blacklist.</param>
        /// <param name="blacklist">list of absolute dir paths to not copy recursively. Exclusive with whitelist.</param>
        /// <exception cref="InvalidOperationException">whitelist and blacklist can't be used simultaneously</exception>
        public void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true, string[] whitelist = null, string[] blacklist = null)
        {
            if (whitelist != null && blacklist != null)
                throw new InvalidOperationException("Can't use both whitelist and blacklist");

            pathWhitelist = null;
            pathBlacklist = null;

            try
            {
                if (whitelist != null)
                    pathWhitelist = BuildSet(whitelist);
                else if (blacklist != null)
                    pathBlacklist = BuildSet(blacklist);

                DirectoryCopyAlgorithm(sourceDirName, destDirName, copySubDirs);
            }
            finally
            {
                //cleanup
                pathWhitelist = null;
                pathBlacklist = null;
            }
        }

        HashSet<string> BuildSet(string[] sourceList)
        {
            var result = new HashSet<string>();
            foreach (var path in sourceList)
            {
                result.Add(path.ToUpperInvariant());
            }
            return result;
        }

        void DirectoryCopyAlgorithm(string sourceDirName, string destDirName, bool overwriteFiles = false, bool copySubDirs = true, bool whitelistOverride = false, bool whitelistContinuation = false)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            //directory structure should be created only when actual whitelisted subdir needs copying
            if (!whitelistContinuation && !Directory.Exists(destDirName))
            {
                //Debug.WriteLine("create dir " + destDirName);
                Directory.CreateDirectory(destDirName);
            }

            //files should not be copied when algorithm scans for lower-level whitelisted subdirs
            if (!whitelistContinuation)
            {
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    //Debug.WriteLine("copying " + file.FullName);
                    string temppath = Path.Combine(destDirName, file.Name);
                    file.CopyTo(temppath, overwriteFiles);
                }
            }

            if (copySubDirs)
            {
                if (pathWhitelist != null)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        //if path is whitelisted, any subpaths and subfiles are whitelisted as well
                        if (whitelistOverride || pathWhitelist.Contains(subdir.FullName.ToUpperInvariant()))
                        {
                            string temppath = Path.Combine(destDirName, subdir.Name);
                            DirectoryCopyAlgorithm(subdir.FullName, temppath, overwriteFiles, copySubDirs, whitelistOverride: true);
                        }
                        //even if its not on whitelist, it's possible subdir is on whitelist
                        else
                        {
                            string temppath = Path.Combine(destDirName, subdir.Name);
                            DirectoryCopyAlgorithm(subdir.FullName, temppath, overwriteFiles, copySubDirs, whitelistContinuation: true);
                        }
                    }
                }
                else if (pathBlacklist != null)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        if (!pathBlacklist.Contains(subdir.FullName.ToUpperInvariant()))
                        {
                            string temppath = Path.Combine(destDirName, subdir.Name);
                            DirectoryCopyAlgorithm(subdir.FullName, temppath, overwriteFiles, copySubDirs);
                        }
                    }
                }
                else
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopyAlgorithm(subdir.FullName, temppath, overwriteFiles, copySubDirs);
                    }
                }
            }
        }
    }
}
