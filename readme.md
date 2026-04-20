# srt4dvd

This .NET 8 console application takes an SRT file and rewrites it into something that DVD authoring software (specifically [DVD Flick](https://www.dvdflick.net/)) will accept without crashing or producing a 25MB coaster.

It exists because:
- Subtitle files found online are... frequently *quirky*
- Legacy DVD tools can be extremely picky
- When quirky subtitle files and legacy DVD tools combine, you frequently get a cut-off video
- Manually fixing 4,500 subtitle entries does not make for a fun afternoon.

So this tool normalizes timing, cleans text, and outputs a DVD-safe SRT file. This gives me more time to read middle-school science books written in Spanish instead. :)

---

## To Use

### Drag & Drop

Drag an `.srt` file onto the executable.

The application will output a new file in the same directory:

```
originalFile-Sanitized.srt
```

---

### Command Line

```
srt4dvd.exe path\to\file.srt
```

---

## Important Notes

- Only supports file paths with **exactly one period total** (the `.srt` extension)
  - 🙂 `C:\movie.srt`  
  - 💩 `C:\movie.en.srt` 
  - 🙂 `C:\thicc-corgi\movie.srt`  
  - 💩 `C:\thicc.corgi\movie.srt`
- Input file must be a valid `.srt` file (basic structure intact)
- Output file will always be: `<originalName>-Sanitized.srt`

This is strict for now, but a future update will address it.

---

## What Is a "Valid" SRT File?

At minimum, an SRT file should follow this structure:

```
1
00:00:01,000 --> 00:00:03,000
Hello world
This is a second hello world line.

2
00:00:04,000 --> 00:00:06,000
This is another subtitle

3
00:00:07,000 --> 00:00:09,000
I want to hug a corgi
```

Each subtitle block consists of:

1. Index number (increments by 1)
2. Timestamp line in this exact format: `HH:MM:SS,mmm --> HH:MM:SS,mmm`
3. One or more lines of text
4. A blank line separating entries

---

## What It Does

- Removes HTML tags (`<i>`, `<b>`, etc.)
- Sanitizes text to remove problematic characters
- Merges nearby subtitle lines (max 3 per block)
- Rebuilds timing:
  - No overlaps
  - Minimum duration enforced
  - Reasonable spacing between subtitles
- Outputs a clean, strictly increasing timeline

In short:
> It takes a questionable SRT and turns it into something that DVD Flick can use.

---

## Workflow

This is how I use it:

1. Find SRT file online  
2. (Optional) Run through Subtitle Edit for basic cleanup  
3. Run `srt4dvd`  
4. Feed output into DVD Flick  
5. Hope I didn’t mess up aspect ratio or audio offsets and have to redo everything  

---

## Background

Modern streaming services make it surprisingly difficult to access content in multiple languages without:
- paying for multiple subscriptions
- dealing with region restrictions
- or just not having the option at all

Meanwhile, DVDs often include multiple audio tracks and subtitles (perfect for learning languages)... but:
- they’re region locked
- tools for working with them are old and fragile

This project exists as part of a larger workflow to:
- extract and normalize subtitles
- combine them with multi-language audio
- produce something that works on my cheap DVD player

---

## Technology Stack

- .NET 8
- Regex (doing most of the heavy lifting here)
- A lot of trial and error with DVD Flick

---

## License

This project is released under CC0 (public domain).

If something breaks:
- fix it
- fork it
- submit a pull request