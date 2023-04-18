import win32com.client
import socket


server = socket.socket()
server.bind(('',8536))
server.listen(1)

def read_cmd(sock):
    cmd = ''

    try:
        while True:
            data = sock.recv(1024).decode('UTF-8')
            cmd += data

            if '\n' in data:
                break

            if len(data) == 0:
                break            

    except:
        pass

    return cmd.replace('\n','')

def cmd_reader(sock):
    Excel = win32com.client.Dispatch("Excel.Application")

    wb = Excel.Workbooks.Open('D:\\Projects\\ВУЗ\\MAI-Laba3\\xl_rec.xls')
    sheet = wb.ActiveSheet
    sheet.ChartObjects("Диаграмма 1").Activate()
    diag = wb.ActiveChart


    val_min = 150
    while True:
        cmd = read_cmd(sock)

        if cmd == '':
            break
        
        cmd_segs = cmd.split('::')

        if cmd_segs[0] == 'enter':
            nums = [float(num) for num in cmd_segs[1].split(':')]

            val_min = min(nums)
            Primary_Axis = diag.Axes(AxisGroup=1)
            Foo_yAxis = Primary_Axis(2)
            Foo_yAxis.MinimumScale = val_min

            for i, num in enumerate(nums):
                sheet.Cells(2+i,1).value = num
                diag.FullSeriesCollection(1).Values = f"=Test!$A$2:$A${i+2}"

        elif cmd_segs[0] == 'clean':
            diag.FullSeriesCollection(1).Values = f"=Test!$A$2:$A$2"

            i = 2
            while True:
                if sheet.Cells(i,1).value == None:
                    break

                sheet.Cells(i,1).value = ""
                i+=1

    #сохраняем рабочую книгу
    wb.Save()

    #закрываем ее
    wb.Close()

    #закрываем COM объект
    Excel.Quit()

while True:
    sock, conn = server.accept()
    print(conn)
    cmd_reader(sock)
    
    
    print('end conn')
    sock.close()