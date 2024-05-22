import sys
from PyQt5.QtWidgets import QApplication, QWidget, QFormLayout, QLineEdit, QHBoxLayout, QVBoxLayout, QPushButton
from PyQt5.QtGui import QDoubleValidator


class FormWidget(QWidget):
    def __init__(self, parent=None):
        super(FormWidget, self).__init__(parent)
        self.initUI()

    def initUI(self):
        self.form_layout = QFormLayout()
        self.line_edits_DrivePositions = []

        # Create a double validator
        double_validator = QDoubleValidator()
        double_validator.setDecimals(2)
        double_validator.setNotation(QDoubleValidator.StandardNotation)
        double_validator.setBottom(0.0)

        for i in range(3):
            row_layout = QHBoxLayout()
            row_edits = []
            for j in range(3):
                match j:
                    case 0:
                        line_edit = QLineEdit(self)
                        line_edit.setPlaceholderText("X")
                        line_edit.setValidator(double_validator)
                        row_edits.append(line_edit)
                        row_layout.addWidget(line_edit)
                    case 1:
                        line_edit = QLineEdit(self)
                        line_edit.setPlaceholderText("Y")
                        line_edit.setValidator(double_validator)
                        row_edits.append(line_edit)
                        row_layout.addWidget(line_edit)
                    case 2:
                        line_edit = QLineEdit(self)
                        line_edit.setPlaceholderText("Z")
                        line_edit.setValidator(double_validator)
                        row_edits.append(line_edit)
                        row_layout.addWidget(line_edit)

            self.line_edits_DrivePositions.append(row_edits)
            self.form_layout.addRow(f"Drive {i} Position:", row_layout)

        self.setLayout(self.form_layout)

    def get_line_edit_values(self):
        values = []
        for row in self.line_edits:
            row_values = [line_edit.text() for line_edit in row]
            values.append(row_values)
        return values


class MainWindow(QWidget):
    def __init__(self):
        super().__init__()
        self.initUI()

    def initUI(self):
        self.main_layout = QVBoxLayout()

        self.form = FormWidget(self)
        self.main_layout.addWidget(self.form)

        self.button = QPushButton("Set Values", self)
        self.button.clicked.connect(self.show_values)
        self.main_layout.addWidget(self.button)

        self.setLayout(self.main_layout)
        self.setWindowTitle('Seilroboter')
        self.show()

    def show_values(self):
        values = self.form.get_line_edit_values()
        print(values)  # You can replace this with any action you need


def main():
    app = QApplication(sys.argv)
    mainWindow = MainWindow()
    sys.exit(app.exec_())


if __name__ == '__main__':
    main()
