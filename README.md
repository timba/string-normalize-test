# String.Normalize test on different platforms

## Intro

This test shows that `String.Normalize` output depends on runtime platform.

As sample, the test application is run on three Docker images:

- Debian based (mcr.microsoft.com/dotnet/core/aspnet:3.1)
- Alpine based (mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine)
- Alpine based (mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine) with ICU installed

## Test description

The test normalizes string `Königsberg` with diacritics using UTF Normalization Form D.
The expected output should change UTF symbol 246 `ö` to normalized form of two symbols:
 - 111 `o`
 - 776 - COMBINING DIAERESIS from UTF Combining Diacritical Marks

 The following Regex replace with `[A-Za-z]` should leave only `o`.

Expectation is transform string `Königsberg` to `Konigsberg`.

## Test outputs

This test works as expected in Debian based Docker image. But on Alpine the test fails because it has default culture set to invariant. This makes `ö` leave as is in normalized string, and then `ö` is removed:

```
Königsberg -> Knigsberg
```

If turn off invariant setting using env variable `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT`:

```
DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

The following runtime error raised:

```
Process terminated. Couldn't find a valid ICU package installed on the system. Set the configuration flag System.Globalization.Invariant to true if you want to run with no globalization support.
   at System.Environment.FailFast(System String)
   at System.Globalization.GlobalizationMode.GetGlobalizationInvariantMode()
   at System.Globalization.GlobalizationMode..cctor()
   at System.Globalization.Normalization.Normalize(System.String, System.Text.NormalizationForm)
   at System.String.Normalize(System.Text.NormalizationForm)
   at normalize_test.Program.Sanitise(System.String, System.String)
   at normalize_test.Program.Main(System.String[])
```

This happens because ICU (International Components for Unicode library) libraries aren't installed in Alpine by default. To fix that, need to install Apline package `icu`:

```bash
apk update && apk add icu
```

After that th test passed successfully.

## Docker fix for Alpine

The fix for Alpine in Dockerfile looks like this:

```Dockerfile
RUN apk update && apk add icu
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
```

## How to run tests

```
docker build .
```

or if elevated permissions required:

```bash
sudo docker build .
```

## Tets results

### Debian

```
Königsberg -> Konigsberg
TEST OK 
```

### Alpine original

```
Königsberg -> Knigsberg
TEST FAILED
```

### Alpine fixed

```
Königsberg -> Konigsberg
TEST OK 
```

