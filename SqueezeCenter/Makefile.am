include $(top_srcdir)/build.rules.mk

ASSEMBLY=SqueezeCenter

FILES = \
	gtk-gui/generated.cs \
	gtk-gui/SqueezeCenter.Configuration.cs \
	src/BrowseMusicItem.cs \
	src/Configuration.cs \
	src/EnqueueCommand.cs \
	src/IconDownloader.cs \
	src/ItemSource.cs \
	src/MusicItem.cs \
	src/NetworkStreamTextReader.cs \
	src/PlayCommand.cs \
	src/PlayerCommands/Next.cs \
	src/PlayerCommands/Pause.cs \
	src/PlayerCommands/Play.cs \
	src/PlayerCommands/PlayerCommand.cs \
	src/PlayerCommands/Prev.cs \
	src/PlayerCommands/Sync.cs \
	src/PlayerCommands/TurnOff.cs \
	src/PlayerCommands/TurnOn.cs \
	src/PlayerCommands/Unsync.cs \
	src/Player.cs \
	src/RadioItem.cs \
	src/Server.cs \
	src/Settings.cs \
	src/SqueezeCenterItem.cs \
	src/Util.cs

RESOURCES = \
	gtk-gui/gui.stetic \
	gtk-gui/objects.xml \
	Resources/icons/radio.png \
	Resources/icons/SB_off.png \
	Resources/icons/SB_on.png \
	Resources/icons/sync.png \
	Resources/icons/unsync.png

GENERATED_RESOURCES = \
	Resources/SqueezeCenter.addin.xml	

REFERENCES = \
	System \
	System.Core \
	$(GTK_SHARP_20_LIBS) \
	$(DO_PLATFORM_LINUX_LIBS) \
	$(DO_UNIVERSE_LIBS)
