# Quickstart GIF for the README.
#
# Renders Samples/Mediarq.Samples.Quickstart running end-to-end, then converts the recording to
# assets/quickstart.gif. Requires https://github.com/charmbracelet/vhs (which needs ttyd + ffmpeg).
#
#   vhs assets/quickstart.tape
#
# then embed it in the README with:
#
#   ![Mediarq quickstart](assets/quickstart.gif)

Output assets/quickstart.gif

Set Shell "bash"
Set FontSize 16
Set Width 1200
Set Height 800
Set Theme "Dracula"
Set Padding 20

Type "dotnet run --project Samples/Mediarq.Samples.Quickstart"
Enter
Sleep 8s
