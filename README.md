# Ressourcenplanung

## Beschreibung

Das System ist dazu bestimmt die Ressourcenallokation zu optimieren. Es können zusätzlich Hilfsmittel für Termine geordert werden. Die beteiligten werden vom System per E-Mail ständig informiert.

## Anforderungen

### Funktional

* Zu bedienen von der Verwaltung des Arbeitsgerichts

* Read-Only-Ansicht für andere Beteiligte

* 

### Fachlich:

* Eine Kalenderansicht 
* Eine Ressourcenansicht
* Eine Terminansicht

### Technisch

* Realisierung des Frontends mit SPA
* Realisierung des Back-Ends mit .net Core und Entity Framework

## Absicherung und Berechtigungen

Die Authentifizierung erfolgt über Windows. Die Authorisierung passiert über das Active Directory.

Es wird mindestens zwei Rollen geben:

**Beobacher:** Diese Nutzer dürfen sich den Kalender ansehen, die Raumliste und können Terminanfragen schicken. 

**Bearbeiter:** Diese Nutzer dürfen selbst Termine erstellen und können Terminanfragen bestätigen.

**Ggf. Admin:** Hier können neue Räume, Vorlagen, Nutzergruppen uvm. erstellt werden. Um das ganze einfacher zu halten würde es sich anbieten diese Rechte auch an Bearbeiter zu vergeben.

#### Berechtigungen

|                      | Beobachter | Bearbeiter | Admin |
| -------------------- | ---------- | ---------- | ----- |
| Termin eintragen     | x          |            |       |
| Termin bestätigen    | x          | x          |       |
| Ressourcen verwalten |            |            | x     |

## Oberfläche

Es gibt keinen Anmeldebildschirm da das Programm inhouse eingesetzt wird. Die Authentifizierung erfolgt im Hintergrund. 

* Es existiert eine Kalendarische Ansicht in verschiedenen Ansichtsstufen.

* Es existiert eine Ressourcen (Raum)-Ansicht.

* Es existiert ein Button oder eine Ansicht zum Stellen von Buchungsanfragen.

* Eine Ansicht zum direkten Erstellen von Terminen ggf. auch als Serientermin bzw. Multitermin mit gleicher Beschreibung. Diese Ansicht sollte bestenfalls in eine andere Ansicht integriert werden und ist dann nur für Berechtigte sichtbar.

* Es existiert eine Ansicht für die Unterstützungskräfte, welche die anstehende Termine für zur Lieferung angeforderter Raumausstattung anzeigt.

* Es existiert eine relativ simple Ansicht zur Verwaltung von (abgelehnten) Anfragen und Entwürfen. Bearbeiter können alle Anfragen sehen. Die Entwürfe sind für deren Ersteller sichtbar. Abgelehnte Entwürfe sind für deren Ersteller sichtbar und können korrigiert werden.

## Cron-Jobs

Die Anwendung kann automatsch kurz vorher Personen benachrichtigungen welche als Ansprechpartner in einen Termin eingetragen sind und Unterstützergruppen kurz vor Terminen welche diese Betreffen.
Diese Funktionen können auch über die WebApi angestuert werden unter
* /api/debug/rememberrequesters - Benachrichtigung der Terminersteller
* /api/debug/remembersupporters - Benachrichtigung der Unterstützergruppen

## E-Mail Versand

Es sollen Benachrichtigungen wenn Ereignisse eintreten welche für die Betroffenen relevant sind (ggf. abschaltbar). 

## Anwendungsfälle für Logik

- [ ] Zeige bestätigte Termine
- [ ] Zeige unbestätigte Termine
- [ ] Zeige Termine pro Ressource
- [ ] Erzeuge Terminanfrage
- [ ] Bestätige Terminanfrage

## Installation

Die Projektmappe muss kompiliert werden mit dem Startprojekt Rema.WebApi, dies erfolgt automatisch über das Skript "build-all" des Web-UI Projekts. Dieses Skript erstellt automatisch eine gezippte Version der produktiven Dateien mit Versionsnummer im Namen und legt sie im Hauptverzeichnis ab.

## Update

Wenn Änderungen im Datenbankschema vorliegen muss die Datenbank aktualisiert werden, die entsprechenden Skripte liegen im Projekt Rema.DbAccess im Ordner Migrations. 
Die Dateien des Webserver-Codes und des WEB-UI müssen überschrieben werden. Änderungen in den Konfigurationen eingetragen werden. Dies ist vor allem die appsettings.json.  

# Kontakt

Oberverwaltungsgericht Rheinland-Pfalz, Deinhardpassage 1, 56068 Koblenz 
Telefon: 0261 1307 - 0
poststelle(at)ovg.jm.rlp.de

# Lizenz

Copyright © 2019-2021 Oberverwaltungsgericht Rheinland-Pfalz 
Lizenziert unter der EUPL, version 1.2 oder höher
Für weitere Details siehe Lizenz.txt oder EUPL-1.2 EN.txt
oder online unter https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12