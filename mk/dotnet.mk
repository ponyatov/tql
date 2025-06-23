.PHONY: dotnet
dotnet: \
	/etc/apt/trusted.gpg.d/microsoft.asc \
	/etc/apt/sources.list.d/microsoft.list
	sudo apt update
	sudo apt install -uy dotnet-runtime-$(DOTNET_VER) dotnet-sdk-$(DOTNET_VER)
	dotnet tool install --global fantomas
/etc/apt/trusted.gpg.d/microsoft.asc:
	sudo $(CURL) $@ $(MS_URL)/keys/microsoft.asc
/etc/apt/sources.list.d/microsoft.list:
	sudo $(CURL) $@ $(MS_URL)/config/debian/12/prod.list
