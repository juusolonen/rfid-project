FROM python:3.7-slim-buster

RUN apt-get update && apt-get install -y cmake udev sqlite3

RUN groupadd -g 995 spi && \
    groupadd -g 993 gpio && \
    groupadd -g 1000 backend && \
    useradd -u 1000 -g backend -G spi,gpio -m -s /bin/bash backend


COPY requirements.txt ./

RUN pip install -r ./requirements.txt

COPY app/ ./app/
COPY MFRC522/ ./app/MFRC522/

RUN mkdir -p /instance && chown backend:backend /instance
RUN mkdir -p /app/data && chown -R backend:backend /app/data && chmod 777 /app/data

USER backend
CMD ["python", "app/app.py"]