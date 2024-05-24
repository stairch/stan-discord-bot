FROM python:3.12-alpine

WORKDIR /opt/stan
EXPOSE 8080
ENTRYPOINT [ "python", "main.py" ]

COPY requirements.txt /opt/stan
RUN pip3 install --no-cache-dir -r requirements.txt

COPY . /opt/stan
