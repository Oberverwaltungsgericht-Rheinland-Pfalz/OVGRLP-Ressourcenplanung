import {MockHandler} from 'vite-plugin-mock-simple'

const today: string = new Date().toISOString()
const tomorrow: string = new Date(Date.now() + 24 * 36e5).toISOString()
const nextWeek: string = new Date(Date.now() + 7 * 24 * 36e5).toISOString()

// todo: ids richtig zuordnen!
export default [
{
    pattern: "/api/Users/me",
    jsonBody: {
        "Roles": [
            {
                "AdDescription": "Admins",
                "Name": "Admin",
                "Level": 100
            },
            {
                "AdDescription": "Readers",
                "Name": "Reader",
                "Level": 1
            },
            {
                "AdDescription": "Editors",
                "Name": "Editor",
                "Level": 10
            }
        ],
        "SupportGroupIds": null,
        "Id": 1,
        "ActiveDirectoryID": "A-1",
        "Name": "Müller, Michael",
        "Organisation": "Oberverwaltungsgericht Rheinland-Pfalz",
        "Email": "michael@müller.de"
    }
},
{
    pattern: "/api/gadgets",
    jsonBody: [
        {
            "Id": 1,
            "Title": "Leinwand",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 2,
            "Title": "Beamer",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 3,
            "Title": "Visualisierer",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 4,
            "Title": "Laptop",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 5,
            "Title": "Presenter",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 6,
            "Title": "Internet",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 7,
            "Title": "Netzwerk",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 8,
            "Title": "Soundsystem",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 9,
            "Title": "Videokonferenzsystem",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 10,
            "Title": "Rücksprache",
            "SuppliedBy": 1,
            "IsDeactivated": false
        },
        {
            "Id": 11,
            "Title": "Kaffee",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 12,
            "Title": "Wasser",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 13,
            "Title": "Gebäck",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 14,
            "Title": "Flipchart",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 15,
            "Title": "Overhead-Projektor",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 16,
            "Title": "Rednerpult",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 17,
            "Title": "Stehtische",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 18,
            "Title": "Tische / Stühle stellen",
            "SuppliedBy": 2,
            "IsDeactivated": false
        },
        {
            "Id": 21,
            "Title": "Benachrichtigen",
            "SuppliedBy": 3,
            "IsDeactivated": false
        }
    ]
},
{
    pattern: "/api/suppliergroups",
    jsonBody: [
        {
            "Id": 1,
            "Title": "EDV",
            "GroupEmail": "edv@gericht.de",
            "Remind": true
        },
        {
            "Id": 2,
            "Title": "Hausverwaltung",
            "GroupEmail": "Hausverwaltung@gericht.de",
            "Remind": true
        },
        {
            "Id": 3,
            "Title": "Infopoint",
            "GroupEmail": "infopoint@gericht.de",
            "Remind": false
        }
    ]
},
{pattern: "/api/ressources",
jsonBody: [
    {
        "Id": 22,
        "Name": "E231",
        "FunctionDescription": "Multifunktionsraum",
        "SpecialsDescription": "großer Plasmabildschirm; bis zu 10 Personen mit Tischen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 23,
        "Name": "Foyer",
        "FunctionDescription": null,
        "SpecialsDescription": null,
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 24,
        "Name": "Foyer des NJZ KO",
        "FunctionDescription": null,
        "SpecialsDescription": null,
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 25,
        "Name": "Konferenzraum E127",
        "FunctionDescription": "Multifunktionsraum",
        "SpecialsDescription": "Videokonferenzsystem; bis zu max. 12 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 26,
        "Name": "B013",
        "FunctionDescription": "Multifunktionsraum",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; individuelle Bestuhlung möglich; bis zu 40 Personen; kann mit B014 zu einer Einheit verbunden werden",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 27,
        "Name": "B014",
        "FunctionDescription": "Multifunktionsraum",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; individuelle Bestuhlung möglich; bis zu 40 Personen; kann mit B013 zu einer Einheit verbunden werden",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 28,
        "Name": "E031",
        "FunctionDescription": "Multifunktionsraum",
        "SpecialsDescription": "Whiteboard; bis zu 20 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 29,
        "Name": "A007",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; bis zu max. 14 Personen; kann mit A008 zu einer Einheit verbunden werden ",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 30,
        "Name": "A008",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; bis zu max. 14 Personen; kann mit A007 zu einer Einheit verbunden werden ",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 31,
        "Name": " A021",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Visualisierer; Klimatisierung; bis zu max. 14 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 32,
        "Name": "A022",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; bis zu max. 14 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 33,
        "Name": "A025",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; bis zu max. 14 Personen; kann mit A026 zu einer Einheit verbunden werden ",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 34,
        "Name": " A026",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Klimatisierung; bis zu max. 14 Personen; kann mit A025 zu einer Einheit verbunden werden ",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 35,
        "Name": "E009",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Visualisierer; Laptop; Klimatisierung; bis zu max. 60 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 36,
        "Name": "E012",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "Deckenbeamer; Leinwand; Laptop; bis zu max. 11 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 37,
        "Name": "E022",
        "FunctionDescription": "Sitzungssaal",
        "SpecialsDescription": "bis zu max. 11 Personen; kein Beamer",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 38,
        "Name": "Sozialraum E401",
        "FunctionDescription": "Multifunktionsraum",
        "SpecialsDescription": "individuelle Bestuhlung möglich; bis zu max. 26 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 52,
        "Name": "C304",
        "FunctionDescription": "Miniraum",
        "SpecialsDescription": "Deckenbeamer;",
        "Type": "Raum",
        "IsDeactivated": false
    },
    {
        "Id": 53,
        "Name": "C314",
        "FunctionDescription": "Beratungsraum",
        "SpecialsDescription": "bis zu max. 10 Personen",
        "Type": "Raum",
        "IsDeactivated": false
    }
]
},
{
    pattern: "/api/allocations",
    jsonBody: [
        {
            "Id": 8528,
            "Title": " Wichtige Staatsprüfung",
            "Notes": "",
            "From": "2024-04-09T08:00:00",
            "To": "2024-04-09T17:00:00",
            "IsAllDay": false,
            "ContactPhone": "+49 111 / 1111 - 33333",
            "Status": 1,
            "RessourceIds": [
                26,
                27
            ],
            "GadgetsIds": [
                18
            ],
            "CreatedById": 15,
            "CreatedAt": "2022-06-08T07:54:20.8301011",
            "LastModified": "2022-06-08T07:54:20.8299639",
            "LastModifiedById": 15,
            "ApprovedById": 15,
            "ApprovedAt": "0001-01-01T00:00:00",
            "ReferencePersonId": "51",
            "ScheduleSeries": "a30a69d1-f3b0-4df9-962c-ad6a287d46a3",
            "HintsForSuppliers": []
        },
        {
            "Id": 8529,
            "Title": " Unwichtige Staatsprüfung",
            "Notes": "",
            "From": "2024-04-11T08:00:00",
            "To": "2024-04-11T17:00:00",
            "IsAllDay": false,
            "ContactPhone": "+49 432 / 1307 - 12345",
            "Status": 1,
            "RessourceIds": [
                26,
                27
            ],
            "GadgetsIds": [
                18
            ],
            "CreatedById": 15,
            "CreatedAt": "2022-06-08T07:54:20.8301011",
            "LastModified": "2022-06-08T07:54:20.8299639",
            "LastModifiedById": 15,
            "ApprovedById": 15,
            "ApprovedAt": "0001-01-01T00:00:00",
            "ReferencePersonId": "51",
            "ScheduleSeries": "a30a69d1-f3b0-4df9-962c-ad6a287d46a3",
            "HintsForSuppliers": []
        },
        {
            "Id": 8530,
            "Title": "Wichtige Besprechung",
            "Notes": "",
            "From": "2024-04-12T08:00:00",
            "To": "2024-04-12T17:00:00",
            "IsAllDay": false,
            "ContactPhone": "+49 111 / 3333 - 10100",
            "Status": 1,
            "RessourceIds": [
                26,
                27
            ],
            "GadgetsIds": [
                18
            ],
            "CreatedById": 15,
            "CreatedAt": "2022-06-08T07:54:20.8301015",
            "LastModified": "2022-06-08T07:54:20.8299643",
            "LastModifiedById": 15,
            "ApprovedById": 15,
            "ApprovedAt": "0001-01-01T00:00:00",
            "ReferencePersonId": "51",
            "ScheduleSeries": "a30a69d1-f3b0-4df9-962c-ad6a287d46a3",
            "HintsForSuppliers": []
        },
        {
            "Id": 8531,
            "Title": "Aufsichtsarbeiten im Aufsehen",
            "Notes": "",
            "From": "2024-04-15T08:00:00",
            "To": "2024-04-15T17:00:00",
            "IsAllDay": false,
            "ContactPhone": "+49 222 / 1311 - 11100",
            "Status": 1,
            "RessourceIds": [
                26,
                27
            ],
            "GadgetsIds": [
                18
            ],
            "CreatedById": 15,
            "CreatedAt": "2022-06-08T07:54:20.8301015",
            "LastModified": "2022-06-08T07:54:20.8299643",
            "LastModifiedById": 15,
            "ApprovedById": 15,
            "ApprovedAt": "0001-01-01T00:00:00",
            "ReferencePersonId": "51",
            "ScheduleSeries": "a30a69d1-f3b0-4df9-962c-ad6a287d46a3",
            "HintsForSuppliers": []
        },
        {
            "Id": 8532,
            "Title": "Schriftliche Urteilsverkündung",
            "Notes": "",
            "From": "2024-04-16T08:00:00",
            "To": "2024-04-16T17:00:00",
            "IsAllDay": false,
            "ContactPhone": "+49 123 / 1234 - 00000",
            "Status": 1,
            "RessourceIds": [
                26,
                27
            ],
            "GadgetsIds": [
                18
            ],
            "CreatedById": 15,
            "CreatedAt": "2022-06-08T07:54:20.8301019",
            "LastModified": "2022-06-08T07:54:20.8299647",
            "LastModifiedById": 15,
            "ApprovedById": 15,
            "ApprovedAt": "0001-01-01T00:00:00",
            "ReferencePersonId": "51",
            "ScheduleSeries": "a30a69d1-f3b0-4df9-962c-ad6a287d46a3",
            "HintsForSuppliers": []
        }
    ]
},
{
    pattern: "/api/Users/Names/{num}",
    jsonBody: []    
},
{
    pattern: "/api/Users/Names/*",
    jsonBody: [
        {
            "Id": 6,
            "Title": "Muster, Max",
            "Email": "muster.max@gericht.de",
            "Organisation": "Oberverwaltungsgericht Rheinland-Pfalz"
        },
        {
            "Id": 15,
            "Title": "Musterfrau, Anna (ArbG Koblenz)",
            "Email": "Maus.Mini@gericht.de",
            "Organisation": "Arbeitsgericht"
        },
    ]
}
] as  MockHandler[]
