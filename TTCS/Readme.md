# TechnicalTestCS

Application de consultation d’articles (liste + détail) avec une page Admin pour gérer un statut (machine à états) persistant en base.

Backend : .NET 8 / ASP.NET Core Web API

Frontend : Vue 3 + Vite + Tailwind

DB : PostgreSQL (Docker)

Observability (optionnel) : Grafana + Loki (Docker)

# Prérequis

Docker (recommandé) + Docker Compose

Node.js (LTS) + npm

(Optionnel) .NET SDK 8 si vous lancez le backend hors Docker

Le repo contient un global.json pour verrouiller la version du SDK .NET.

#  Démarrage rapide (recommandé)
#  Lancer le backend + PostgreSQL (Docker)

Depuis la racine du repo :

docker compose up -d --build

API dispo sur : http://localhost:8080

Swagger : http://localhost:8080/swagger

# Lancer le frontend

Dans le dossier frontend/technicaltestcs-front :

npm install

npm run dev

Front dispo sur : http://localhost:5173

Le front proxy automatiquement /api vers http://localhost:8080.

Observability optionnelle (Grafana + Loki)

#  Docker  :

docker-compose.yml : api + postgres > docker compose up -d --build

Stop > docker compose down

docker-compose.obs.yml : loki + grafana + activation Loki côté API

Lancer avec observability

Depuis la racine du repo :

docker compose -f docker-compose.yml -f docker-compose.obs.yml up -d --build

docker compose -f docker-compose.yml -f docker-compose.obs.yml up -d loki grafana

Stop > docker compose -f docker-compose.yml -f docker-compose.obs.yml down

# Grafana 

http://localhost:3000 (admin / admin)

Créer une datasource loki > lier avec cette url > http://loki:3100 

Dans notre cas l'authentification sera sans paramètres mais l'on peut ajouter un username/password ( ou token ). 

Ces informations sont à repercuter dans l'appsettings si ce choix est fait.

Après quelques modifications d'articles sur le front, aller dans Grafana → Explore → datasource Loki > Explore data

requête exemple :

{service="technicaltestcs-api"}

# Tests

Depuis backend/ :

dotnet test

Notes sur la machine à états

Statuts :

draft (par défaut)

pending

accepted

rejected

Transitions autorisées :

draft → pending

pending → accepted

pending → rejected

rejected → pending

Toute transition non autorisée renvoie une erreur 400.

# Structure du repo (simplifiée)

backend/ : API .NET + persistance EF Core

frontend/technicaltestcs-front/ : app Vue 3

docker-compose.yml : api + postgres

docker-compose.obs.yml : loki + grafana (optionnel)