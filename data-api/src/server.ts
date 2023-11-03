/**
 * Setup express server.
 */
import express, { Request, Response } from 'express';
import config from './config';

import 'express-async-errors';
import findDoors from './doors';

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

// Nav to users pg by default
app.get('/image', (req: Request, res: Response) => {
  return res.status(501).json({ error: { message: 'not yet implemented' }})
});

app.listen(config.port, () => console.log('server started'));
