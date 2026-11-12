using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TimerPomodoro;

internal sealed record PomodoroState(int Total, int Today, int Yesterday, DateOnly Date) {
    public PomodoroState AdvanceTo(DateOnly date) => date == Date
        ? this
        : this with { Today = 0, Yesterday = date.DayNumber - Date.DayNumber == 1 ? Today : 0, Date = date };
}

internal static class PomodoroStore {
    private const string FILENAME = "TimerPomodoro.txt";
    private static readonly string DIRECTORYPATH = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimerPomodoro");
    internal static string FilePath => Path.Combine(DIRECTORYPATH, FILENAME);

    internal static (PomodoroState State, bool NeedsSave) Load() {
        string[] Paths = [FilePath, Path.GetFullPath(FILENAME), Path.Combine(AppContext.BaseDirectory, FILENAME)];
        foreach (string Path in Paths.Distinct(StringComparer.OrdinalIgnoreCase)) {
            string[] Lines;
            try {
                Lines = File.ReadAllLines(Path);
            }
            catch (FileNotFoundException) {
                continue;
            }
            catch (DirectoryNotFoundException) {
                continue;
            }

            if (Lines.Length != 4
                || !int.TryParse(Lines[0], NumberStyles.None, CultureInfo.InvariantCulture, out int Total)
                || !int.TryParse(Lines[1], NumberStyles.None, CultureInfo.InvariantCulture, out int Today)
                || !int.TryParse(Lines[2], NumberStyles.None, CultureInfo.InvariantCulture, out int Yesterday)
                || (long)Today + Yesterday > Total
                || !TryReadDate(Lines[3], out DateOnly Date)) {
                throw new FormatException($"The saved counters in '{Path}' are invalid. The file has not been changed.");
            }

            return (new(Total, Today, Yesterday, Date), !StringComparer.OrdinalIgnoreCase.Equals(Path, FilePath));
        }

        return (new(0, 0, 0, DateOnly.FromDateTime(DateTime.Today)), false);
    }

    private static bool TryReadDate(string text, out DateOnly date) {
        if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) {
            return true;
        }

        if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime LegacyDate)) {
            date = DateOnly.FromDateTime(LegacyDate);
            return true;
        }

        return false;
    }

    internal static void Save(PomodoroState state) {
        Directory.CreateDirectory(DIRECTORYPATH);
        string TemporaryPath = $"{FilePath}.{Guid.NewGuid():N}.tmp";
        using (FileStream Stream = new(TemporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
            using StreamWriter Writer = new(Stream, new UTF8Encoding(false), leaveOpen: true);
            Writer.WriteLine(state.Total.ToString(CultureInfo.InvariantCulture));
            Writer.WriteLine(state.Today.ToString(CultureInfo.InvariantCulture));
            Writer.WriteLine(state.Yesterday.ToString(CultureInfo.InvariantCulture));
            Writer.WriteLine(state.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            Writer.Flush();
            Stream.Flush(true);
        }

        if (File.Exists(FilePath)) {
            File.Replace(TemporaryPath, FilePath, null);
        }
        else {
            File.Move(TemporaryPath, FilePath);
        }
    }
}
