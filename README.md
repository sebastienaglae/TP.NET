# README - Projet Bibliothèque en ligne - AGLAE & CHIAPPE

## Auteurs
- Sébastien AGLAE
- Mike CHIAPPE

## Description
Ce projet vise à développer un service web accompagné d'un client Windows (WPF) pour la gestion et la consultation d'une bibliothèque de livres. Il permet aux utilisateurs d'ajouter, supprimer, et consulter des livres et leurs genres. Le projet intègre une interface d'administration web, une API REST pour les interactions serveur, et une application Windows pour la consultation des livres.  

## Informations de connexion
- **Nom d'utilisateur**: `Admin`
- **Mot de passe**: `mbds`

## Technologies utilisées
- **Langages**: C#, HTML, Javascript, CSS, TypeScript
- **Serveur web**: ASP.Net Core
- **Client Windows**: WPF
- **Gestion des données**: Entity Framework Core (InMemory)

## Structure du projet
- **Library.sln**: Solution contenant l'ensemble du projet.
- **BookLibrary.Server**: Projet contenant le serveur web.
- **BookLibrary.ServerPluginKit**: Projet contenant le kit de développement pour les plugins serveur (C#).
- **BookLibrary.Client**: Projet contenant le client Windows.
- **BookLibrary.Client.OpenApi**: Projet contenant l'interface de l'API REST.

## Fonctionnalités
### Administration
- Ajout, suppression et modification de livres dans la bibliothèque.
- Consultation de la liste des livres, genres, et auteurs (avec pagination).
- Ajout de nouveaux genres.
- Authentification basique avec un compte administrateur prédéfini.
- Options avancées incluant la gestion des auteurs, le filtrage par titre/auteurs/genres, statistiques diverses, et l'importation de détails de livres via OpenLibrary.

### API REST
- Listing des livres avec pagination et filtrage par genre et/ou auteur.
- Listing des genres disponibles avec pagination.
- Listing des auteurs disponibles avec pagination.
- Récupération des détails d'un livre par son identifiant (avec les détails et pages).
- Support de plugins pour ajouter des routes et des fonctionnalités avancées (support assembly managé & non-managé avec chargement et déchargement dynamique).

### Application Windows
- Lister les livres avec détails et option de lecture.
- Filtrer les livres par genres et/ou auteurs (Plusieurs genres et plusieurs auteurs).
- Lecture du livre via l'API System.Speech.SpeechSynthesizer avec support de lecture, pause, et arrêt + lire a partir de la séléction de l'utilisateur et suivis de la progression.
- Pagination (simple bouton)
- Fluent style App ([WPF-UI](https://github.com/lepoco/wpfui)  from LePoco)

## Troubleshooting

Si ASP ne démarre pas avec l'erreur suivante : `Unhandled exception. System.InvalidOperationException: Path /hello is already mapped`, supprimer le dossier `plugins` dans le projet `BookLibrary.Server` et relancer le serveur.
