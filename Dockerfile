# --- Phase 1: Build ---
# Utilise l'image du SDK .NET 9 pour construire l'application (basé sur votre arborescence)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copie le fichier solution et le fichier projet pour optimiser la mise en cache des calques Docker.
# L'étape 'restore' ne sera ré-exécutée que si ces fichiers changent.
COPY War.sln .
COPY War/War.csproj War/

# Restaure les paquets NuGet pour toute la solution
RUN dotnet restore War.sln

# Copie le reste du code source du projet
COPY War/. War/

# Publie l'application en mode Release. La sortie sera dans /app/publish
# On utilise --no-restore car la restauration a déjà été faite.
RUN dotnet publish "War/War.csproj" -c Release -o /app/publish --no-restore

# --- Phase 2: Finale ---
# Utilise l'image runtime ASP.NET, plus légère et optimisée pour les applications web/network.
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copie uniquement le résultat de la publication depuis la phase de build
COPY --from=build /app/publish .

# Expose le port sur lequel le serveur WebSocket écoute à l'intérieur du conteneur
EXPOSE 8080

# Définit la commande à exécuter au démarrage du conteneur
ENTRYPOINT ["dotnet", "War.dll"]