# TaskManagerAPI - Test Tecnico

## Obiettivo

Implementa una Minimal API in .NET 8 che consenta di gestire task multi-tenant, salvando i dati in un file JSON locale, con particolare attenzione alla sicurezza dei dati.

## Funzionalità richieste

- POST /tasks
- GET /tasks (con filtraggio tramite header `X-Tenant-ID`
- Salvataggio dei dati in file JSON

## Funzionalità facoltative

- PUT /tasks/{id}
- Test unitari

## Extra

- Per ogni task crea un record di dati nel nostro applicativo. 

Valorizza i seguenti campi (fieldName):

	1. TASK_ID (String 255)
	2. TASK_DESCRIPTION (String 255)
	3. CREATION DATE (Date)

Al seguente link trovi lo swagger: https://services.paloalto.swiss:10443/api2/swagger/index.html

Di seguito, le informazioni necessarie per l'autenticazione e la valorizzazione dei metadati:

- User ID: 23
- Password WS: 1234
- Cabinet ID: 804dfcb0-cf00-49c7-bb23-ec68bc3a6097

## Istruzioni

- Puoi modificare la struttura del progetto come preferisci
- Usa solo file system (niente database)
- Inserisci le tue risposte nel file `README.md` alla fine

## Domande finali

1. Hai riscontrato difficoltà? Dove?
2. Hai fatto assunzioni? Se sì, quali?
3. Come miglioreresti il codice se fosse un progetto reale?
4. Hai usato strumenti di supporto (AI, StackOverflow, ecc)? Se sì, come?
