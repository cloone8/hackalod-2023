/**
 * Setup express server.
 */
import express, { Request, Response } from 'express';
import config from './config';

import 'express-async-errors';
import findDoors from './doors';
import { getArtworksByPainter } from './artworks';

// **** Variables **** //

const app = express();

app.use(express.json());

app.get('/status', (req: Request, res: Response) => {
  return res.status(200).json({ message: 'I\'m alive!'})
})

app.get('/doors', async (req: Request, res: Response) => {
  await findDoors("bla");

  return res.status(200).json({});
})

app.get('/artworks', async (req: Request, res: Response) => {
  const { painterId } = req.query
  if (!painterId || !(typeof painterId == 'string')) {
    res.status(400).json({ error: { message: 'missing queryparam "painterId"'}})
    return
  }
  const string = await getArtworksByPainter(painterId)
  res.json(string)
})

// Nav to users pg by default
app.get('/image', (req: Request, res: Response) => {
  return res.status(501).json({ error: { message: 'not yet implemented' }})
});

app.listen(config.port, () => console.log('server started'));
