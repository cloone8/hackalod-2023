/**
 * Setup express server.
 */
import express, { Request, Response } from 'express';

import 'express-async-errors';

// **** Variables **** //

const app = express();

app.use(express.json());

// Nav to users pg by default
app.get('/image', (req: Request, res: Response) => {
  return res.redirect('/users');
});

app.listen(3000, () => console.log('server started'));