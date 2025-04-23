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

## Istruzioni

- Puoi modificare la struttura del progetto come preferisci
- Usa solo file system (niente database)
- Inserisci le tue risposte nel file `README.md` alla fine

## Commento (Aggiunto da me)
Sono riuscito a completare tutte le task richieste (obbligatorie, facoltative ed extra: la creazione della task viene salvata su DocuWare).
Per organizzare meglio il codice, l’ho suddiviso in questi moduli:

- Cartella data: contiene il file JSON (tasks.json) utilizzato per salvare le task in locale.

- Cartella Models: contiene il modello TaskItem.

- Progetto JuniorSoftwareDeveloper.Tests: contiene i test unitari realizzati con xUnit.

## Domande finali

1. Hai riscontrato difficoltà? Dove?

- Deserializzazione in LoadTasks(): se il file JSON era vuoto, il tentativo di deserializzazione di una nuova task generava un’eccezione. Ho risolto avvolgendo il parsing in un blocco try–catch che, in caso di errore o JSON vuoto, restituisce una lista vuota.

- Binding del progetto di test: configurare il progetto di test per farlo puntare correttamente al progetto principale non è stato immediato; ho dovuto aggiungere i riferimenti e controllare il namespace.

- Integrazione con DocuWare: per la chiamata a /query-documents ricevevo 400 Bad Request finché, analizzando lo Swagger, non ho scoperto che anche per le query GET era necessario inviare un payload con le credenziali. Inoltre, dato che lo Swagger restituiva 401 Unauthorized, ho creato un endpoint interno (/tasks/records) che dopo aver aggiunto la task via POST se chiamato restituisce la lista dei documenti per verificare l’effettiva presenza tramite GET.

- Formato del payload DocuWare: inizialmente sbagliavo la struttura dei campi (indexFields), ma dopo vari test e confronti con la documentazione, ho individuato il formato corretto.
   
2. Hai fatto assunzioni? Se sì, quali?

- Nell’endpoint PUT ho deciso di consentire anche l’aggiornamento del TenantId, ipotizzando che una task possa essere riassegnata a un altro tenant.

Per DocuWare, ho dato per scontato che:

- add-record inserisca un nuovo documento nel cabinet.
- query-documents restituisca l’elenco completo dei documenti presenti.

3. Come miglioreresti il codice se fosse un progetto reale?

Se fosse un progetto reale migliorerei:

- Migrazione delle credenziali (userId, passwordWS, cabinetId) in variabili d’ambiente o in User Secrets/appsettings.json protetti.

- Garantire che tutte le chiamate esterne avvengano sempre su HTTPS, con validazione dei certificati.

- Error handling avanzato: sostituire EnsureSuccessStatusCode() con log dettagliati ed eccezioni personalizzate, restituendo all’API client messaggi chiari e codici di stato appropriati.

- Sostituire il file JSON con un database per scalabilità e query complesse. 

- Estrarre la logica di accesso ai dati locali e la comunicazione con DocuWare in service class separate.

- Ampliare i test unitari per coprire ogni casistica.


4. Hai usato strumenti di supporto (AI, StackOverflow, ecc)? Se sì, come?

- AI (ChatGPT): soprattutto per configurare il binding del progetto di test e approfondire dettagli su DocuWare.

- StackOverflow e documentazione Microsoft: per risolvere errori di deserializzazione, comprendere i formatter JSON e approfondire il funzionamento dei metodi PostAsJsonAsync, EnsureSuccessStatusCode(), ecc.
