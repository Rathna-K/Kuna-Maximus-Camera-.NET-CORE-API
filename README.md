# Kuna-Maximus-Camera-.NET-CORE-API
API for downloading videos from Kuna Maximus.

# Usage: 
All methods need to be Awaited. 

```
Dim kunaapi As New KunaAPI.KunaAPI(username, password)
kunaapi.authenticate
kunaapi.update 'will update all cameras and their info
'loop thru cameras to get recordings
for each c in camera
    dim recordings = kapi.getallrecordings(c)
    for each recording in recordings
        'get download link
        dlink = kunaapi.getdownloadlink(recording)
    next
next
```
credit to python developer:  marthoc/pykuna
