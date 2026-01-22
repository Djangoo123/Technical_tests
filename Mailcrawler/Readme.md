# Mail crawler (emails mailto:)

Crawler qui récupère les emails (liens mailto:) d’une page HTML et des pages liées, avec une profondeur maximale.

# Prérequis

.NET 8

# Lancer les tests
dotnet test

# Points clés

Parcours en largeur (BFS) pour explorer d’abord les pages proches (depth 0 → 1 → 2…)

Emails et pages dédoublonnés (pas de boucle infinie si les pages se referencent entre elles)

maximumDepth = -1 : exploration sans limite de profondeur

# Tests

Les TU couvrent :

l’exemple donné (depth 0/1/2)

le mode illimité (-1)

un cas de cycle

un mailto: avec querystring (?subject=...)

un lien avec fragment (#...)