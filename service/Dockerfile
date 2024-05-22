FROM python:3.12-alpine

WORKDIR /opt/stan

ENTRYPOINT [ "python", "main.py" ]

EXPOSE 8080
COPY . /opt/stan
RUN pip3 install --no-cache-dir -r requirements.txt
