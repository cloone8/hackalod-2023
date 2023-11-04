import express, { Request, Response } from 'express';
import config from './config';
import query, { entityTypes } from './sparql';
import sharp from 'sharp';

import 'express-async-errors';

const app = express();

app.use(express.json());

app.get('/status', (_req: Request, res: Response) => {
  return res.status(200).json({ message: 'I\'m alive!'})
})

app.get('/entity/:entityType/:entityId', async (req: Request, res: Response) => {
  const { entityType, entityId } = req.params;

  if (!entityTypes.includes(entityType)) {
    return res.status(400).json({ error: `Invalid entity type ${entityType}. Expected one of [${entityTypes.join(', ')}]`})
  }

  const result = await query(entityType, entityId);

  return res.status(200).json(result);
})

app.get(`/image/:url`, async (req: Request, res: Response) => {
  const { url } = req.params;

  const response = await fetch(url);

  const contentType = response.headers.get('Content-Type');
  const input = await response.arrayBuffer()

  const resized = await sharp(input).resize(null, 1024).toBuffer()

  return res.status(200).contentType(contentType!).end(resized);
})

app.listen(config.port, () => console.log('server started'));
