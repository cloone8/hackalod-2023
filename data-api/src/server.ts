import express, { Request, Response } from 'express';
import config from './config';

import 'express-async-errors';
import findDoors, { EntityType } from './doors';
import { getArtworksByCity, getArtworksByPainter } from './artworks';

const entityTypes = ['artist'];

const app = express();

app.use(express.json());

app.get('/status', (req: Request, res: Response) => {
  return res.status(200).json({ message: 'I\'m alive!'})
})

app.get('/:entityType/:entityId', async (req: Request, res: Response) => {
  const { entityType, entityId } = req.params;

  if (!entityTypes.includes(entityType)) {
    return res.status(400).json({ error: `Invalid entity type ${entityType}. Expected one of [${entityTypes.join(', ')}]`})
  }

  const doors = await findDoors(entityType as EntityType, entityId);

  return res.status(200).json(doors);
})

app.get('/artworks', async (req: Request, res: Response) => {
  const { painterId, cityId } = req.query
  if (painterId && typeof painterId == 'string') {
    const string = await getArtworksByPainter(painterId)
    res.json(string)
    return
  }
  if (cityId && typeof cityId == 'string') {
    const string = await getArtworksByCity(cityId)
    res.json(string)
    return
  }
  res.status(400).send('Missing painter or city id.')
})

// Nav to users pg by default
app.get('/image', (req: Request, res: Response) => {
  return res.status(501).json({ error: { message: 'not yet implemented' }})
});

app.listen(config.port, () => console.log('server started'));
