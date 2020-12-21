/*
 * Created by SharpDevelop.
 * User: Ahmed
 * Date: 11/21/2020
 * Time: 3:15 PM
 * 
 * This script takes an input folder and a backup folder
 * it starts by moving all files not in the excluded extensions
 * into the backup folder, then deletes every empty directory
 * that is left.
 * 
 * Be sure to change the consts and statics
 * it also creates the backup dirs on its own
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace HelloWorld
{
	public struct ListofFiles
	{
		public List<string> Files;
		public List<string> Exts;
		
		public ListofFiles(List<string> emplist, List<string> nolist)
		{
			this.Files = emplist;
			this.Exts = nolist;
		}
	}
	
	class Program
	{
		const string testdir = "D:\\Music\\";
		const string backupdir = "D:\\music_backup\\";

		static string[] wantedext = new string[]{ ".mp3", ".m3u", ".wma" };
		static string[] exculdefolder = new string[]{ "lyrics" };

		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			
			DeleteAllFiles(testdir, backupdir);
			DeleteEmptyDirsRecurse(testdir);
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
				
		public static List<string> RemoveExculusions(string[] allfiles)
		{
			List<string> result = new List<string>();
			foreach (var file in allfiles) {
				bool has_exc = false;
				foreach (var exfold in exculdefolder) {
					if (file.Contains(exfold)) {
						has_exc = true;
						break;
					}
				}
				if (!has_exc)
					result.Add(file);
			}
			return result;
		}
		
		static List<string>GetEmptyDirs(string toppath)
		{
			string[] alldirs = Directory.GetDirectories(toppath, "*", SearchOption.AllDirectories);
			var alldirsrefined = RemoveExculusions(alldirs);
			List<string> emptydirs = new List<string>();

			foreach (string dir in alldirsrefined) {
				var files = Directory.GetFiles(dir);
				var dirs = Directory.GetDirectories(dir);
				if (files.Length + dirs.Length == 0) // Empty dir
					emptydirs.Add(dir);
			}
			
			return emptydirs;
		}

		static ListofFiles GetNonMP3Files(string toppath)
		{
			List<string> result = new List<string>();
			List<string> foundext = new List<string>();
			
			var allfiles = Directory.GetFiles(toppath, "*", SearchOption.AllDirectories);
			var allfilesrefined = RemoveExculusions(allfiles);

			foreach (var file in allfilesrefined) {
				var ext = Path.GetExtension(file).ToLower();
				bool hasmp3 = false;
				foreach (var mp3ext in wantedext) {
					if (ext == mp3ext) {
						hasmp3 = true;
						break;
					}
				}
				if (!hasmp3) {
					result.Add(file);
					if (!foundext.Contains(ext))
						foundext.Add(ext);
				}
			}
			return new ListofFiles(result, foundext);
		}
		
		static void DeleteEmptyDirsRecurse(string Path)
		{
			int deleted_count = 0;
			int level_count = 0;
			while(true)
			{
				Console.WriteLine("Starting level " + level_count.ToString());
				
				var emptydirs = GetEmptyDirs(Path);
				if (emptydirs.Count == 0)
					break;
				else
				{
					foreach (var dir in emptydirs)
					{
						Console.WriteLine("Deleting " + dir);
						Directory.Delete(dir);
						deleted_count++;
					}
				}
				
				Console.WriteLine();
				level_count++;
			}
			
			Console.WriteLine();
			Console.WriteLine(string.Format("Finished, deleted {0} folders, took {1} levels", deleted_count, level_count));
			Console.WriteLine();
		}
		
		static void DeleteAllFiles(string toppath, string backuppath)
		{
			var files = GetNonMP3Files(toppath);
			
			outputlist(files.Files);
			outputlist(files.Exts);
			Console.WriteLine();
			Console.WriteLine("Files and exts that will be deleted are above");
			Console.WriteLine("Press anything yo...");
			Console.ReadKey();
			Console.WriteLine();
			
			foreach(var file in files.Files)
			{
				// move them to backup dir
				// top path ends with \
				Console.WriteLine(string.Format("Backing up {0}", file));

				string final_path = file.Replace(toppath, backuppath);
				Directory.CreateDirectory(Path.GetDirectoryName(final_path));
				File.Move(file, final_path);
				File.SetAttributes(final_path, FileAttributes.Normal);
			}
			Console.WriteLine();
			Console.WriteLine(string.Format("Finished backing up {0} files", files.Files.Count));
			Console.WriteLine();
		}
		
		static void outputlist(List<string> list)
		{
			foreach (var s in list)
				Console.WriteLine(s);
		}

	}
}