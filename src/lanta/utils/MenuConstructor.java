package lanta.utils;

import lanta.math.Parser;

import java.util.HashMap;
import java.util.Scanner;

public class MenuConstructor {
    private final HashMap<Integer, String> options;
    private final boolean showDefault;

    public MenuConstructor(String defaultOption,String... options) {
        this(true, defaultOption, options);
    }

    public MenuConstructor(boolean showDefault, String defaultOption,String... options) {
        this.options = new HashMap<>();
        this.showDefault = showDefault;
        this.options.put(0, defaultOption);
        int option1 = 1;
        for (String option : options) {
            this.options.put(option1, option);
            option1++;
        }
    }

    public int getOption(Scanner scanner) {
        StringBuilder message = new StringBuilder();
        if(showDefault) message.append(0 + " - ").append(options.get(0));
        for(int i = 1; i < options.size(); i++){
            message.append(" | ").append(i).append(" - ").append(options.get(i));
        }
        System.out.println(message);
        System.out.println("Select an option:");
        int selectedOption = Parser.toNumber(scanner.nextLine(), Integer::parseInt, 0);
        if(options.containsKey(selectedOption)) return selectedOption;
        else {
            System.out.println("Invalid option, defaulting to 0 - " + options.get(0));
            return 0;
        }
    }
}
