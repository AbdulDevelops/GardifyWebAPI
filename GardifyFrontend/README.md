Start project: (localhost:4200)
ng serve

Build project for deployment: (output in gardify/dist)
ng build --prod --output-hashing=all
copy files to Webs/gardify.sslbeta.de

Create component:
ng g component components/[componentName]

Create service:
ng g service services/[serviceName]

publish to serve:
ng build --prod 
- in terminal located in folder of project

Copy "gardify"-folder located in "dist" folder to the server.
Make backup folder of gardify.sslbeta.de in Übertragung-Folder

Replace all files in gardify.sslbeta.de except "web.config" with uploaded files
