import express from 'express'
const app = express()
const port = 3000

app.get('/', (req, res) => {
  res.send('Hello World!')
})

app.get('/topic', async (req, res) => {
  res.send(await getNext(req.query.url as string))
})

app.post('/configure', async (req, res) => {
  configure(req.body as Configuration)
})

app.listen(port, () => {
  return console.log(`Express is listening at http://localhost:${port}`)
})
