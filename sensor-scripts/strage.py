import sqlite3

class Strage:
    conn = None

    def  __init__(self, db_path) :
        print("connect sqlite3 database... db="+db_path)
        self.conn = sqlite3.connect(db_path)
        cur = self.conn.cursor()

        cur.execute(
        """
        CREATE TABLE IF NOT EXISTS thermals(
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            measured_at TEXT,
            host_name TEXT,
            temperature REAL,
            humidity REAL,
            api_status_code INT
        )
        """
        )

    def close(self):
        self.conn.close()

    def store(self, date, hostname, temprature, humidity, status_code):
        cur = self.conn.cursor()

        cur.execute(
            """
            INSERT INTO thermals(measured_at,host_name,temperature,humidity, api_status_code)
            VALUES(
                '{0}',
                '{1}',
                {2},
                {3},
                {4}
            )
            """.format(date, hostname, temprature, humidity, status_code)
        )
        self.conn.commit()