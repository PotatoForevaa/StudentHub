import { createContext } from "react";

export type BurgerContextType = {
    isOpen: boolean
    setMenu: (menu: boolean) => void;
}

export const BurgerContext = createContext<BurgerContextType>({
    isOpen: false,
    setMenu: () => { }
});