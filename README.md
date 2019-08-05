# Raumplanungssystem

## Beschreibung

Das System ist dazu bestimmt die Verwaltung der Räume im Haus zu planen und zu steuern

## Anforderungen

### Funktional

* Zu bedienen von der Verwaltung des Arbeitsgerichts

* Read-Only-Ansicht für andere Beteiligte

* 

### Fachlich:

* Eine Kalenderansicht 

### Technisch

* Realisierung des Frontends mit SPA

* Realisierung des Back-Ends mit .net Core

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

## E-Mail Versand

Es sollen Benachrichtigungen wenn Ereignisse eintreten welche für die Betroffenen relevant sind (ggf. abschaltbar). 

## Anwendungsfälle für Logik

- [ ] Zeige bestätigte Termine

- [ ] Zeige unbestätigte Termine

- [ ] Zeige Termine pro Ressource

- [ ] Erzeuge Terminanfrage

- [ ] Bestätige Terminanfrage

- [ ] 
