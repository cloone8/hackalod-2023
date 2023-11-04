export default (data: any[] | undefined) => {
  if (!data) {
    return [];
  }

  return data
    .filter((row) => row.paintingurl && row.paintingname)
    .map((row) => ({
      label: row.paintingname?.value,
      desc: row.paintingdesc?.value,
      url: row.paintingurl?.value,
    }));
};
